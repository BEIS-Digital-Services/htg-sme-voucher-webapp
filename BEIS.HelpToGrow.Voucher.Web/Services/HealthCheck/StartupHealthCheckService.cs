using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class StartupHealthCheckService : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
        {
            var isHealthy = true;
            await Task.FromResult(0);
            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Help to grown web app is healthy");
            }

            return new HealthCheckResult(
                    context.Registration.FailureStatus, "Help to grown web is unhealthy");
        }
    }
}
