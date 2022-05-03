using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace BEIS.HelpToGrow.Voucher.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .ConfigureLogging(_ =>
                        {
                            _.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Debug);
                            _.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
                        });
                });
    }
}
