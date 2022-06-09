using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class CompanyHouseHealthCheckService : IHealthCheck
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly ICompanyHouseHttpConnection<CompanyHouseResponse> _companyHouseHttpConnection;
        private readonly IOptions<CompanyHouseHealthCheckConfiguration> _companyHouseHealthCheckOptions;

        public CompanyHouseHealthCheckService(
            IRestClientFactory iRestClientFactory,
            ILogger<CompanyHouseHealthCheckService> logger,
            IConfiguration iConfiguration,
            ICompanyHouseHttpConnection<CompanyHouseResponse> companyHouseHttpConnection, 
            IOptions<CompanyHouseHealthCheckConfiguration> companyHouseHealthCheckOptions)
        {
            _logger = logger;
            _companyHouseHttpConnection = companyHouseHttpConnection;
            _companyHouseHealthCheckOptions = companyHouseHealthCheckOptions;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                var response = _companyHouseHttpConnection.ProcessRequest(_companyHouseHealthCheckOptions.Value.CompanyNumber, new DefaultHttpContext());

                if (response.CompanyName == _companyHouseHealthCheckOptions.Value.CompanyName 
                    && response.CompanyStatus == _companyHouseHealthCheckOptions.Value.CompanyStatus)
                {
                    isHealthy = true;
                    _logger.LogError("Company house API health check passed.");
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
