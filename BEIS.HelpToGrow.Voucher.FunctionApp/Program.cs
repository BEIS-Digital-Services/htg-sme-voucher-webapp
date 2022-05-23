using BEIS.HelpToGrow.Voucher.FunctionApp.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.FunctionApp
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            IConfiguration configuration = null;
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults((hostBuilderContext, workerApplicationBuilder) =>
                {
                    workerApplicationBuilder.UseFunctionExecutionMiddleware();
                })
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configuration = configurationBuilder.Build();
                    var connectionString = configuration.GetConnectionString("AppConfig");
                    if (connectionString != null)
                    {
                        configurationBuilder.AddAzureAppConfiguration(connectionString);
                    }
                })
                .ConfigureLogging((context, loggerBuilder) =>
                {
                    loggerBuilder.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.RegisterFunctionAppServices(configuration);                    
                })
                .Build();


            return host.RunAsync();
        }
    }
}