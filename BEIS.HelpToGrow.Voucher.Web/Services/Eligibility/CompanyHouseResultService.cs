
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class CompanyHouseResultService : ICompanyHouseResultService
    {
        private readonly ICompanyHouseResultRepository _repository;
        private readonly ILogger<CompanyHouseResultService> _logger;

        public CompanyHouseResultService(ICompanyHouseResultRepository repository, ILogger<CompanyHouseResultService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> SaveAsync(CompanyHouseResponse response)
        {
            try
            {
                var resultAlreadyExists = await _repository.Exists(response.CompanyNumber);

                if (resultAlreadyExists)
                {
                    return Result.Ok();
                }

                var apiResult = new companies_house_api_result();
                apiResult.company_name = response.CompanyName;
                apiResult.company_number = response.CompanyNumber;
                apiResult.company_status = response.CompanyStatus;
                apiResult.date_of_creation = DateTime.TryParse(response.CreationDate, out var creationDate)
                    ? creationDate
                    : DateTime.Now;
                apiResult.type = response.CompanyType;
                apiResult.has_insolvency_history = response.HasInsolvencyHistory;
                apiResult.jurisdiction = response.Jurisdiction;
                apiResult.locality = response.RegisteredOfficeAddress?.Locality;
                apiResult.registered_office_address = new companies_house_registered_office_address
                    {
                        address_line_1 = response.RegisteredOfficeAddress?.AddressLine1,
                        address_line_2 = response.RegisteredOfficeAddress?.AddressLine2,
                        postal_code = response.RegisteredOfficeAddress?.PostalCode,
                        locality = response.RegisteredOfficeAddress?.Locality,
                        country = response.RegisteredOfficeAddress?.Country
                    };
                apiResult.registered_office_is_in_dispute = response.RegisteredOfficeDisputed;
                apiResult.undeliverable_registered_office_address = response.UndeliverableRegisteredOfficeAddress;
                apiResult.sic_codes = string.Join(",", response.SicCodes ?? Array.Empty<string>());

                await _repository.SaveCompanyHouseResult(apiResult);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving companies house response");
                
                return Result.Fail(ex.Message);
            }
        }
    }
}