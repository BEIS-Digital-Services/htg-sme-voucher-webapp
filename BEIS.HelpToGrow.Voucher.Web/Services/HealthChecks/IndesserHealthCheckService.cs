using Newtonsoft.Json;

namespace Beis.HelpToGrow.Voucher.Web.Services.HealthChecks
{
    public class IndesserHealthCheckService : HealthCheckServiceBase
    {
        private readonly ILogger<IndesserHealthCheckService> _logger;
        private readonly IIndesserHttpConnection<IndesserCompanyResponse> _indesserHttpConnection;
        private readonly IOptions<CompanyHouseHealthCheckConfiguration> _companyHouseHealthCheckOptions;
        private readonly ICheckEligibility _eligibility;

        public IndesserHealthCheckService(ILogger<IndesserHealthCheckService> logger,
            IIndesserHttpConnection<IndesserCompanyResponse> indesserHttpConnection,
            IOptions<CompanyHouseHealthCheckConfiguration> companyHouseHealthCheckOptions, ICheckEligibility eligibility, IApplicationInsightsPublisher applicationInsightsPublisher)
            : base(applicationInsightsPublisher, logger)
        {
            _logger = logger;
            _indesserHttpConnection = indesserHttpConnection;
            _companyHouseHealthCheckOptions = companyHouseHealthCheckOptions;
            _eligibility = eligibility;
        }

        public override async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult result;
            var isHealthy = true;

            var indesserCheckErrors = new StringBuilder();
            try
            {
                var indesserCallResult = RunIndesserCheck(_companyHouseHealthCheckOptions.Value.CompanyNumber);

                if (indesserCallResult.IsFailed)
                {
                    indesserCheckErrors.Append(indesserCallResult.Errors[0].Message);
                    isHealthy = false;

                }
                else
                {

                    var eligibilityCalculation = _eligibility.Check(new UserVoucherDto { }, indesserCallResult.Value);

                    if (eligibilityCalculation.IsFailed)
                    {
                        indesserCheckErrors.Append(eligibilityCalculation.Errors[0].Message);
                        isHealthy = false;
                    }
                }
            } catch (Exception ex)
            {
                isHealthy = false;                
                _logger.LogError(ex, $"Indesser health check failed. Its failed with {ex.Message}");
            }

            if (isHealthy)
            {
                result = HealthCheckResult.Healthy("Indesser service health check passed.");
            }
            else
            {
                result = new HealthCheckResult(
                    context.Registration.FailureStatus, $"Indesser service health check failed. Its failed with {indesserCheckErrors}");
            }
            await base.LogHealthCheckResult(context, result);
            return result;
                
        }

        private Result<IndesserCompanyResponse> RunIndesserCheck(string companyHouseNumber)
        {
            
            var indesserCheckResult = _indesserHttpConnection.ProcessRequest(companyHouseNumber, new DefaultHttpContext());

            if (indesserCheckResult.IsFailed)
            {
                _logger.LogWarning(JsonConvert.SerializeObject(indesserCheckResult.Errors));
            }

            return indesserCheckResult;
        }
    }
}
