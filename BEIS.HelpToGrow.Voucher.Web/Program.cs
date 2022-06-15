using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;


namespace Beis.HelpToGrow.Voucher.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration(configuration =>
                {
                    var connectionString = configuration.Build().GetConnectionString("AppConfig");
                    if (connectionString != null)
                    {
                        configuration.AddAzureAppConfiguration(connectionString);
                    }
                }).ConfigureLogging(_ =>
                {
                    _.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Debug);
                    _.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
                });
        }
    }
}