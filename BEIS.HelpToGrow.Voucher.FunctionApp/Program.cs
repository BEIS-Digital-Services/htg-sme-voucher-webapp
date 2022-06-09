using Beis.HelpToGrow.Voucher.FunctionApp.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

IConfiguration configuration = null;
var host = new HostBuilder()
     //.ConfigureFunctionsWorkerDefaults()
     .ConfigureFunctionsWorkerDefaults((hostBuilderContext, workerApplicationBuilder) =>
     {
         workerApplicationBuilder.UseFunctionExecutionMiddleware();
     })
    .ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        //Assembly asm = Assembly.GetExecutingAssembly();
        var connectionString = configurationBuilder.AddUserSecrets<Program>().Build().GetConnectionString("AppConfig");
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
    .Build();

host.Run();