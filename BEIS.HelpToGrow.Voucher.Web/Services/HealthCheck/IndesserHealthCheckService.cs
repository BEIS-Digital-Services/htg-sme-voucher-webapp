using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class IndesserHealthCheckService : IHealthCheck
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly IIndesserHttpConnection<IndesserCompanyResponse> _indesserHttpConnection;
        private readonly IOptions<CompanyHouseHealthCheckConfiguration> _companyHouseHealthCheckOptions;

        public IndesserHealthCheckService(ILogger<CompanyHouseHealthCheckService> logger, 
            IIndesserHttpConnection<IndesserCompanyResponse> indesserHttpConnection, 
            IOptions<CompanyHouseHealthCheckConfiguration> companyHouseHealthCheckOptions)
        {
            _logger = logger;
            _indesserHttpConnection = indesserHttpConnection;
            _companyHouseHealthCheckOptions = companyHouseHealthCheckOptions;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            var indesserCallResult = RunIndesserCheck(_companyHouseHealthCheckOptions.Value.CompanyNumber);

            if (indesserCallResult.IsFailed)
            {
                isHealthy = false;
            }

            if (isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Indesser service health check passed."));
            }

            return
                Task.FromResult(new HealthCheckResult(
                    context.Registration.FailureStatus, "Indesser service health check failed."));
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
