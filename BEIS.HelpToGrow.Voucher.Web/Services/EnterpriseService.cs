
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        private readonly ILogger<EnterpriseService> _logger;
        private readonly IEnterpriseRepository _repository;
        private readonly IEnterpriseSizeRepository _enterpriseSizeRepository;
        private readonly ISessionService _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        private readonly IFCASocietyService _fcaSocietyService;

        public EnterpriseService(
            IEnterpriseRepository repository,
            IEnterpriseSizeRepository enterpriseSizeRepository,
            ISessionService sessionService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EnterpriseService> logger,
            IProductRepository productRepository,
            IFCASocietyService fcaSocietyService)
        {
            _repository = repository;
            _enterpriseSizeRepository = enterpriseSizeRepository;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _productRepository = productRepository;
            _fcaSocietyService = fcaSocietyService;
        }

        private async Task MapVoucherToEnterprise(UserVoucherDto userVoucher, enterprise enterprise)
        {
            FCASociety society = null;
            if (userVoucher.HasFCANumber.ToBoolean() && !string.IsNullOrWhiteSpace(userVoucher.FCANumber))
            {
                society = await _fcaSocietyService.GetSociety(userVoucher.FCANumber);
            }

            enterprise.enterprise_name = userVoucher.CompanyHouseResponse?.CompanyName ?? society?.SocietyName ?? throw new NullReferenceException("Missing both Companies House name and Society name");
            enterprise.new_tech_ind = userVoucher.FirstTime.ToBoolean();
            enterprise.companies_house_no = userVoucher.HasCompanyHouseNumber.ToBoolean() ? (userVoucher.CompanyHouseResponse?.CompanyNumber ?? userVoucher.CompanyHouseNumber) : null;
            enterprise.fca_no = userVoucher.HasFCANumber.ToBoolean() ? userVoucher.FCANumber : null;
            enterprise.enterprise_size_id = await GetEnterpriseSizeRecord(userVoucher.EmployeeNumbers.ToString());
            enterprise.company_postcode = userVoucher.HasCompanyHouseNumber.ToBoolean() ? userVoucher.CompanyHouseResponse?.RegisteredOfficeAddress?.PostalCode : society?.SocietyAddress.Split(",").ToList().Last();
            if(enterprise.applicant_email_address != userVoucher.ApplicantDto.EmailAddress)
            {
                enterprise.applicant_email_verified = false;
                userVoucher.ApplicantDto.IsVerified = false;
                _sessionService.Set("userVoucherDto", userVoucher, _httpContextAccessor.HttpContext);
            }
            enterprise.applicant_email_address = userVoucher.ApplicantDto.EmailAddress;
            enterprise.applicant_name = userVoucher.ApplicantDto.FullName;
            enterprise.applicant_role = userVoucher.ApplicantDto.Role;
        }

        private async Task<long> GetEnterpriseSizeRecord(string companySize)
        {
            var record =
                await _enterpriseSizeRepository.GetEnterpriseSizeRecord(companySize) ??
                await _enterpriseSizeRepository.AddEnterpriseSize(new enterprise_size { enterprise_size_desc = companySize });
            
            return record.enterprise_size_id;
        }

        private async Task<Result> CreateAsync(UserVoucherDto userVoucher)
        {
            try
            {
                var enterprise = new enterprise
                {
                    enterprise_created_date = DateTime.Now,
                    eligibility_status_id = (long) EligibilityStatus.Unknown,
                    applicant_email_verified = false,
                    agreed_tandc = true,
                    marketing_consent = userVoucher.ApplicantDto.HasProvidedMarketingConsent 
                };

                await MapVoucherToEnterprise(userVoucher, enterprise);
                await _repository.AddEnterprise(enterprise);
                
                var dto = _sessionService.Get<UserVoucherDto>("userVoucherDto", _httpContextAccessor.HttpContext);
                dto.ApplicantDto.EnterpriseId = enterprise.enterprise_id;
                
                _sessionService.Set("userVoucherDto", dto, _httpContextAccessor.HttpContext);
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem creating the enterprise");

                return Result.Fail(ex.Message);
            }
        }

        private async Task<Result> UpdateEnterpriseDetailsAsync(UserVoucherDto userVoucher)
        {
            if (userVoucher.ApplicantDto.EnterpriseId <= 0)
            {
                return Result.Fail($"Invalid enterprise id: '{userVoucher.ApplicantDto.EnterpriseId}'");
            }

            var enterprise = await _repository.GetEnterprise(userVoucher.ApplicantDto.EnterpriseId);

            if (enterprise == null)
            {
                return Result.Fail("The enterprise could not be found.");
            }

            await MapVoucherToEnterprise(userVoucher, enterprise);

            await _repository.UpdateEnterprise(enterprise);

            _sessionService.Set("userVoucherDto", userVoucher, _httpContextAccessor.HttpContext);

            return Result.Ok();
        }

        public async Task<Result> CreateOrUpdateEnterpriseDetailsAsync(UserVoucherDto userVoucher)
        {
            // update only the information provided up to the email verification (before the indesser checks)
            return userVoucher.ApplicantDto.EnterpriseId > 0
                ? await UpdateEnterpriseDetailsAsync(userVoucher)
                : await CreateAsync(userVoucher);
        }

        public async Task<Result> SetEnterpriseAsVerified(long enterpriseId)
        {
            try
            {
                var enterprise = await _repository.GetEnterprise(enterpriseId);
                enterprise.applicant_email_verified = true;
                await _repository.UpdateEnterprise(enterprise);
                var dto = _sessionService.Get<UserVoucherDto>("userVoucherDto", _httpContextAccessor.HttpContext);
                dto.ApplicantDto.IsVerified = true;
                _sessionService.Set("userVoucherDto", dto, _httpContextAccessor.HttpContext);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying the enterprise");

                return Result.Fail(ex.Message);
            }
        }

        public async Task<EligibilityStatus> GetEligibilityStatusAsync()
        {
            long enterpriseId = -1;

            try
            {
                var userVoucher = _sessionService.Get<UserVoucherDto>("userVoucherDto", _httpContextAccessor.HttpContext);

                if (userVoucher is null)
                {
                    return EligibilityStatus.Unknown;
                }

                enterpriseId = userVoucher.ApplicantDto?.EnterpriseId ?? enterpriseId;

                if (enterpriseId < 0)
                {
                    return EligibilityStatus.Unknown;
                }

                var enterprise = await _repository.GetEnterprise(enterpriseId);

                return (EligibilityStatus)enterprise.eligibility_status_id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting eligibility status for enterprise id: {enterpriseId}");

                throw;
            }
        }

        public async Task Unsubscribe(long enterpriseId, string emailAddress)
        {
            try
            {
                var enterprise = await _repository.GetEnterprise(enterpriseId);

                if (enterprise is null)
                {
                    return;
                }

                if (!enterprise.applicant_email_address.Trim().Equals(emailAddress.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                enterprise.marketing_consent = false;

                await _repository.UpdateEnterprise(enterprise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unsubscribing {emailAddress} for enterprise id: {enterpriseId}");
            }
        }

        public async Task<Result> SetEligibilityStatusAsync(EligibilityStatus eligibility)
        {
            long enterpriseId = -1;

            try
            {
                var userVoucher = _sessionService.Get<UserVoucherDto>("userVoucherDto", _httpContextAccessor.HttpContext);

                enterpriseId = userVoucher?.ApplicantDto?.EnterpriseId ?? enterpriseId;

                var enterprise = await _repository.GetEnterprise(enterpriseId);

                enterprise.eligibility_status_id = (long) eligibility;

                await _repository.UpdateEnterprise(enterprise);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting eligibility status to {eligibility} for enterprise id: {enterpriseId}");

                return Result.Fail(ex.Message);
            }
        }

        private async Task<UserVoucherDto> GetUserVoucherFromEnterpriseAsync(long enterpriseId)
        {
            var enterprise = await _repository.GetEnterprise(enterpriseId);
            
            if (enterprise == null)
            {
                return null;
            }

            var userVoucher = _sessionService.Get<UserVoucherDto>("userVoucherDto", _httpContextAccessor.HttpContext) ?? new UserVoucherDto();

            userVoucher.CompanyHouseResponse ??= new CompanyHouseResponse();
            userVoucher.CompanyHouseResponse.RegisteredOfficeAddress ??= new RegisteredOfficeAddress();
            userVoucher.ApplicantDto.EnterpriseId = enterpriseId;
            userVoucher.CompanyHouseResponse.CompanyName = enterprise.enterprise_name;
            userVoucher.FirstTime = enterprise.new_tech_ind.ToString();
            userVoucher.CompanyHouseResponse.CompanyNumber = enterprise.companies_house_no;
            userVoucher.CompanyHouseNumber = enterprise.companies_house_no;
            userVoucher.HasCompanyHouseNumber = userVoucher.CompanyHouseNumber == null ? "No" : "Yes";
            userVoucher.FCANumber = enterprise.fca_no;
            userVoucher.HasFCANumber = userVoucher.FCANumber == null ? "No" : "Yes";
            userVoucher.CompanyHouseResponse.RegisteredOfficeAddress.PostalCode = enterprise.company_postcode;
            var companySizeDescription = (await _enterpriseSizeRepository.GetEnterpriseSizeRecordById(enterprise.enterprise_size_id))?.enterprise_size_desc;
            userVoucher.EmployeeNumbers = !int.TryParse(companySizeDescription, out var companySize)
                ? userVoucher.EmployeeNumbers
                : companySize;
            userVoucher.ApplicantDto.EmailAddress = enterprise.applicant_email_address;
            userVoucher.ApplicantDto.FullName = enterprise.applicant_name;
            userVoucher.ApplicantDto.Role = enterprise.applicant_role;
            userVoucher.ApplicantDto.IsVerified = enterprise.applicant_email_verified;
           
            _sessionService.Set("userVoucherDto", userVoucher, _httpContextAccessor.HttpContext);

            return userVoucher;
        }

        public async Task<UserVoucherDto> GetUserVoucherFromEnterpriseAsync(long enterpriseId, long productId)
        {
            var userVoucher = await GetUserVoucherFromEnterpriseAsync(enterpriseId);
            userVoucher.SelectedProduct = await _productRepository.GetProductSingle(productId);
            _sessionService.Set("userVoucherDto", userVoucher, _httpContextAccessor.HttpContext);

            return userVoucher;
        }

        public async Task<bool> CompanyNumberIsUnique(string companyNumber)
        {
            var enterprise = await _repository.GetEnterpriseByCompanyNumber(companyNumber);

            return enterprise == null;
        }

        public async Task<bool> FcaNumberIsUnique(string fcaNumber)
        {
            var fcaSociety = await _repository.GetEnterpriseByFCANumber(fcaNumber);

            return fcaSociety == null;
        }
    }
}