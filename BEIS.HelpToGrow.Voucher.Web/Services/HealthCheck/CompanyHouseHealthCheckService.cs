using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class CompanyHouseHealthCheckService : IHealthCheck
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly ICompanyHouseHttpConnection<CompanyHouseResponse> _companyHouseHttpConnection;
        
        public CompanyHouseHealthCheckService(
            IRestClientFactory iRestClientFactory,
            ILogger<CompanyHouseHealthCheckService> logger,
            IConfiguration iConfiguration, 
            ICompanyHouseHttpConnection<CompanyHouseResponse> companyHouseHttpConnection)
        {
            _logger = logger;
            _companyHouseHttpConnection = companyHouseHttpConnection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            var companyHouseDetails = new CompanyHouseResponse
            {
                CompanyName = "GTM UK LTD",
                CompanyNumber = "04856708",
                CompanyStatus = "Dissolved"
            };

            try
            {
                var response = _companyHouseHttpConnection.ProcessRequest(companyHouseDetails.CompanyNumber, new DefaultHttpContext());

                if (response.CompanyName == companyHouseDetails.CompanyName 
                    && response.CompanyStatus == companyHouseDetails.CompanyStatus)
                {
                    isHealthy = true;
                    _logger.LogError("Company house API health check passed.");
                } 
            }
            catch (Exception e)
            {
                isHealthy = false;
                _logger.LogError(e, "Company house API health check failed.");
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
