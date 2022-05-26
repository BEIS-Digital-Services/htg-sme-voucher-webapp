using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database.Models;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors;
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
        private readonly IRestClientFactory _iRestClientFactory;
        private readonly IConfiguration _iConfiguration;

        public CompanyHouseHealthCheckService(
            IRestClientFactory iRestClientFactory, 
            ILogger<CompanyHouseHealthCheckService> logger,
            IConfiguration iConfiguration)
        {
            _logger = logger;
            _iRestClientFactory = iRestClientFactory;
            _iConfiguration = iConfiguration;
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

            var companyAPI = new CompanyHouseConnection(
                    _iRestClientFactory,
                    _iConfiguration["COMPANY_HOUSE_URL"],
                    _iConfiguration["COMPANY_HOUSE_API_KEY"],
                    _iConfiguration["VoucherSettings:connectionTimeOut"]);

            try
            {
                var response = companyAPI.ProcessRequest(companyHouseDetails.CompanyNumber, new DefaultHttpContext());

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
