using System.Text.Json;

namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly INotifyService _notifyService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly IEncryptionService _encryptionService;
        private string _salt = string.Empty;

        public EmailVerificationService(
            ILogger<EmailVerificationService> logger,
            INotifyService notifyService,
            IEnterpriseService enterpriseService,
            IEncryptionService encryptionService,
            IConfiguration configuration)
        {
            _logger = logger;
            _notifyService = notifyService;
            _enterpriseService = enterpriseService;
            _encryptionService = encryptionService;
            _salt = configuration["EmailVerificationSalt"];
        }

        public string GetVerificationCode(UserVoucherDto userVoucher)
        {
            var verificationDetails = new EmailVerificationModel
            {
                EnterpriseId = userVoucher.ApplicantDto.EnterpriseId,
                EmailAddress = userVoucher.ApplicantDto.EmailAddress,
                ProductId = userVoucher.SelectedProduct.product_id
            };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(verificationDetails);
            return HttpUtility.HtmlEncode(_encryptionService.Encrypt(jsonString, _salt));
        }

        public async Task<Result> CreateOrUpdateEnterpriseDetailsAsync(UserVoucherDto userVoucher)
        {
            _logger.LogInformation("Executing EmailVerificationService.CreateOrUpdateEnterpriseDetailsAsync at {@time} for company house number {@companyHouseNumber} and fca {@fca}", DateTime.Now, userVoucher.CompanyHouseNumber, userVoucher.FCANumber);
            try
            {
                // update only the information provided up to the email verification (before the indesser checks)
                return await _enterpriseService.CreateOrUpdateEnterpriseDetailsAsync(userVoucher);
            }
            finally
            {
                _logger.LogInformation("EmailVerificationService.CreateOrUpdateEnterpriseDetailsAsync completed at {@time} for company house number {@companyHouseNumber} and fca {@fca}", DateTime.Now, userVoucher.CompanyHouseNumber, userVoucher.FCANumber);
            }            
        }

        public async Task<Result> SendVerifyEmailNotificationAsync(ApplicantDto applicant, string templateId = null)
        {
            _logger.LogInformation("Executing EmailVerificationService.SendVerifyEmailNotificationAsync at {@time} for enterprise {@enterpriseId}", DateTime.Now, applicant.EnterpriseId);
            try
            {
                return await _notifyService.SendVerifyEmailNotification(applicant);
            }
            finally
            {
                _logger.LogInformation("EmailVerificationService.SendVerifyEmailNotificationAsync completed at {@time} for enterprise {@enterpriseId}", DateTime.Now, applicant.EnterpriseId);
            }
            
        }

        public async Task<Result> VerifyEnterpriseFromCodeAsync(string verificationCode)
        {
            try
            {
                _logger.LogInformation("Attempting to verify email address for {0}", verificationCode);
                var verificationDetails = GetVerificationDetailsFromCode(verificationCode);

                if (verificationDetails.EnterpriseId < 1)
                    throw new ArgumentException("The voucher does not contain a valid Id.");
                // confirm that code matches the dto
               
                var  dto = await _enterpriseService.GetUserVoucherFromEnterpriseAsync(verificationDetails.EnterpriseId, verificationDetails.ProductId);
                if (dto == null)
                    return Result.Fail("The enterprise could not be found.");
                if (dto.ApplicantDto.EmailAddress != verificationDetails.EmailAddress)
                    return Result.Fail("The email address does not match.");
                if (dto.ApplicantDto.EnterpriseId != verificationDetails.EnterpriseId)
                    return Result.Fail(new Error("the Id does not match."));

                return await _enterpriseService.SetEnterpriseAsVerified(verificationDetails.EnterpriseId);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There was a problem verifying the applicant address for code {0}", verificationCode);
                return Result.Fail(e.Message);
            }        
        }

        public async Task<bool> CompanyNumberIsUnique(string companyNumber, string fcaNumber)
        {
            _logger.LogInformation("Executing EmailVerificationService.CompanyNumberIsUnique at {@time} for company house number {@companyHouseNumber} and fca {@fca}", DateTime.Now, companyNumber, fcaNumber);
            try
            {
                return !string.IsNullOrWhiteSpace(fcaNumber)
                ? await _enterpriseService.FcaNumberIsUnique(fcaNumber)
                : await _enterpriseService.CompanyNumberIsUnique(companyNumber);
            }
            finally
            {
                _logger.LogInformation("EmailVerificationService.CompanyNumberIsUnique completed at {@time} for company house number {@companyHouseNumber} and fca {@fca}", DateTime.Now, companyNumber, fcaNumber);
            }
        }

        private EmailVerificationModel GetVerificationDetailsFromCode(string verificationCode)
        {
            var resultString = _encryptionService.Decrypt(verificationCode, _salt);
            _logger.LogInformation("result string : {0} decrypted from verification code : {1}", resultString, verificationCode);
            return JsonSerializer.Deserialize<EmailVerificationModel>(HttpUtility.HtmlDecode(resultString));
        }

        public async Task<UserVoucherDto> GetUserVoucherFromEnterpriseAsync(long enterpriseId, long productId)
        {
            
            var dto = await _enterpriseService.GetUserVoucherFromEnterpriseAsync(enterpriseId, productId);
            return dto;
            
        }
    }
}