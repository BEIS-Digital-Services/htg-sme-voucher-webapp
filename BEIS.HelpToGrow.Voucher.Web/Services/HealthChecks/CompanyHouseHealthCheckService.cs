
namespace Beis.HelpToGrow.Voucher.Web.Services.HealthChecks
{
    public class CompanyHouseHealthCheckService : HealthCheckServiceBase
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly ICompanyHouseHttpConnection<CompanyHouseResponse> _companyHouseHttpConnection;
        private readonly IOptions<CompanyHouseHealthCheckConfiguration> _companyHouseHealthCheckOptions;

        public CompanyHouseHealthCheckService(            
            ILogger<CompanyHouseHealthCheckService> logger,            
            ICompanyHouseHttpConnection<CompanyHouseResponse> companyHouseHttpConnection, 
            IOptions<CompanyHouseHealthCheckConfiguration> companyHouseHealthCheckOptions,
            IApplicationInsightsPublisher applicationInsightsPublisher)
            : base(applicationInsightsPublisher, logger)
        {
            _logger = logger;
            _companyHouseHttpConnection = companyHouseHttpConnection;
            _companyHouseHealthCheckOptions = companyHouseHealthCheckOptions;
        }

        public override Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                var response = _companyHouseHttpConnection.ProcessRequest(_companyHouseHealthCheckOptions.Value.CompanyNumber, new DefaultHttpContext());

                if (response.CompanyName != null && response.CompanyName == _companyHouseHealthCheckOptions.Value.CompanyName 
                    && response.CompanyStatus == _companyHouseHealthCheckOptions.Value.CompanyStatus)
                {
                    _logger.LogError("Company house API health check passed.");
                } else
                {
                    isHealthy = false;
                    _logger.LogError("Company house API health check failed. Check company details.");
                }
            }
            catch (Exception e)
            {
                isHealthy = false;
                _logger.LogError(e, "Company house API health check failed. Check company details.");
            }

            if (isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Company house API health check passed."));
            }

            return
                Task.FromResult(new HealthCheckResult(
                    context.Registration.FailureStatus, "Company house API health check failed."));

        }
    }
}
