namespace Beis.HelpToGrow.Voucher.Web.Extensions
{
    public static class HealthCheckExtensions
    {
        internal static void RegisterHealthcheckServices(this IServiceCollection services)
        {

            services.AddHealthChecks()
              .AddCheck<DependencyInjectionHealthCheckService>(
                HealthCheckConstants.DependencyInjectionName,
                HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    HealthCheckType.LowFrequency.ToString(),                  
                    HealthCheckType.DI.ToString()            
                })
              .AddCheck<IndesserHealthCheckService>(
                HealthCheckConstants.IndesserName,
                HealthStatus.Unhealthy,
                tags: new[] 
                {
                    HealthCheckType.LowFrequency.ToString(),
                    HealthCheckType.IO.ToString(),
                    HealthCheckType.Indesser.ToString()
                })
              .AddCheck<CompanyHouseHealthCheckService>(
                HealthCheckConstants.CompaniesHouseName,
                HealthStatus.Unhealthy,
                tags: new[] 
                {
                    HealthCheckType.LowFrequency.ToString(),
                    HealthCheckType.IO.ToString(),
                    HealthCheckType.CompaniesHouse.ToString()
                })
              .AddCheck<DatabaseHealthCheckService>(
                HealthCheckConstants.DatabaseName,
                HealthStatus.Unhealthy,
                tags: new[] 
                {
                    HealthCheckType.HighFrequency.ToString(),
                    HealthCheckType.IO.ToString(),
                    HealthCheckType.Database.ToString()
                })
              .AddCheck<EncryptionHealthCheckService>(
                HealthCheckConstants.EncryptionName,
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] 
                {
                    HealthCheckType.HighFrequency.ToString(),
                    HealthCheckType.Encryption.ToString()
                });
            services.AddScoped<IApplicationInsightsPublisher, ApplicationInsightsPublisher>();

        }
        internal static void MapSMEHealthChecks(this Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            // base healthcheck
            endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = (check) => false,
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/low-frequency", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.LowFrequency.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/high-frequency", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.HighFrequency.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/io", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.IO.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/encryption", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.Encryption.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/database", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.Database.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/di", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.DI.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            endpoints.MapHealthChecks("/healthz/companies-house", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.CompaniesHouse.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            endpoints.MapHealthChecks("/healthz/indesser", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(HealthCheckType.Indesser.ToString()),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            endpoints.MapHealthChecks("/healthz/all", new HealthCheckOptions()
            {
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

        }
    }
}
