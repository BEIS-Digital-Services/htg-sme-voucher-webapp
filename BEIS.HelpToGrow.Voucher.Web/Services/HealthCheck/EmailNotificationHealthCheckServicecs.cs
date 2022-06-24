
namespace Beis.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class EmailNotificationHealthCheckServicecs : IHealthCheck
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly IEmailVerificationService _emailVerificationService;        
        private readonly IOptions<UrlOptions> _options;

        public EmailNotificationHealthCheckServicecs(ILogger<CompanyHouseHealthCheckService> logger, IEmailVerificationService emailVerificationService, IOptions<UrlOptions> options)
        {
            _logger = logger;
            _emailVerificationService = emailVerificationService;        
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                var userVoucherDto = new UserVoucherDto();
                userVoucherDto.ApplicantDto ??= new ApplicantDto();
                userVoucherDto.ApplicantDto.EmailAddress = ""; // TODO: If we need to add email varification service then add a test email address here
                userVoucherDto.ApplicantDto.FullName = ""; // TODO: add a test full name here
                userVoucherDto.ApplicantDto.EnterpriseId = 1;
                userVoucherDto.SelectedProduct ??= new product { product_id = 11 };                                  


            var verificationCode = _emailVerificationService.GetVerificationCode(userVoucherDto);

                userVoucherDto.ApplicantDto.EmailVerificationLink = GetVerificationLink(verificationCode, _options.Value.EmailVerificationUrl);

                var result = await _emailVerificationService.SendVerifyEmailNotificationAsync(userVoucherDto.ApplicantDto);

                if (result.IsFailed)
                {
                    isHealthy = false;
                }

            } catch (Exception ex)
            {
                isHealthy = false;
                string message = $"Notify health check failed. Its faild with {ex.Message}";
                _logger.LogError(ex, message);
            }

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Notify health check failed.");
            }

            return
                new HealthCheckResult(
                    context.Registration.FailureStatus, "Notify health check failed.");
        }
        private static string GetVerificationLink(string verificationCode, string path)
        {
            var param = new Dictionary<string, string> { { "verificationCode", verificationCode } };

            return new Uri(QueryHelpers.AddQueryString(path, param)).ToString();
        }
    }
}
