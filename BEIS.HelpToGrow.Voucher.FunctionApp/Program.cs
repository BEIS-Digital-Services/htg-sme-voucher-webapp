using System.IO;
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
            return new HostBuilder()
                .ConfigureFunctionsWorkerDefaults((hostBuilderContext, workerApplicationBuilder) =>
                {
                    workerApplicationBuilder.UseFunctionExecutionMiddleware();
                })
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    var connectionString =  configurationBuilder.Build().GetConnectionString("AppConfig");
                    if (connectionString != null)
                    {
                        configuration = configurationBuilder.AddAzureAppConfiguration(connectionString).Build();
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
                .Build().RunAsync();
        }
    }
}