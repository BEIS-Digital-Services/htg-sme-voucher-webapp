using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class EncryptionHealthCheckService : IHealthCheck
    {
        private readonly ILogger<EncryptionHealthCheckService> logger;
        private readonly IEncryptionService encryptionService;

        public EncryptionHealthCheckService(ILogger<EncryptionHealthCheckService> logger, IEncryptionService encryptionService)
        {
            this.logger = logger;
            this.encryptionService = encryptionService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                var encryptedString = encryptionService.Encrypt("healthcheck", "healthz");
                var encryptionResult = encryptionService.Decrypt(encryptedString, "healthz");
                if (encryptionResult != "healthcheck")
                {
                    isHealthy = false;
                    logger.LogError("Encryption Healthcheck failed.");
                }

            }
            catch (Exception e)
            {
                isHealthy = false;
                logger.LogError(e, "Encryption Healthcheck failed.");
            }

            if (isHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Encryption Healthcheck passed."));
            }

            return Task.FromResult(
                new HealthCheckResult(
                    context.Registration.FailureStatus, "Encryption Healthcheck failed."));
        }
    }
}
