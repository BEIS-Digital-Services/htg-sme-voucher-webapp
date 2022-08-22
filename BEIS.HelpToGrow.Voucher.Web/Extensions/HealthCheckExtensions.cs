namespace Beis.HelpToGrow.Voucher.Web.Extensions
{
    public static class HealthCheckExtensions
    {
        internal static void RegisterHealthcheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
              .AddCheck<DependencyInjectionHealthCheckService>(
                "Dependency Injection Health Checks",
                HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    "LowFrequency", 
                    "IO", 
                    "DI" 
                })
              .AddCheck<IndesserHealthCheckService>(
                "Indesser Service Health Checks",
                HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    "LowFrequency", 
                    "IO", 
                    "Indesser" 
                })
              .AddCheck<CompanyHouseHealthCheckService>(
                "Company House Api",
                HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    "LowFrequency", 
                    "CompaniesHouse" 
                })
              .AddCheck<DatabaseHealthCheckService>(
                "Database",
                HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    "HighFrequency", 
                    "IO", 
                    "Database" 
                })
              .AddCheck<EncryptionHealthCheckService>(
                "Encryption",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] 
                { 
                    "HighFrequency", 
                    "Encryption" 
                });

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
                Predicate = (check) => check.Tags.Contains("LowFrequency"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/high-frequency", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("HighFrequency"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/io", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("IO"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/encryption", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("Encyrption"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/database", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("Database"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

            endpoints.MapHealthChecks("/healthz/di", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("DI"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            endpoints.MapHealthChecks("/healthz/companies-house", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("CompaniesHouse"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });
            endpoints.MapHealthChecks("/healthz/indesser", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("Indesser"),
                ResponseWriter = HealthCheckJsonResponseWriter.Write
            });

        }
    }
}
