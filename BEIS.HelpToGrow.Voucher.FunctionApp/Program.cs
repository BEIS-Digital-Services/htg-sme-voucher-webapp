using System;
using System.IO;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using BEIS.HelpToGrow.Voucher.FunctionApp.Extensions;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Config;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using BEIS.HelpToGrow.Voucher.Web.Services.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.FunctionApp
{
    public class Program
    {
        //public static Task Main(string[] args)
        //{
        //    IConfiguration configuration = null;
        //    var host = new HostBuilder()
        //        .ConfigureFunctionsWorkerDefaults((hostBuilderContext, workerApplicationBuilder) =>
        //        {
        //            workerApplicationBuilder.UseFunctionExecutionMiddleware();
        //        })
        //        .ConfigureAppConfiguration((context, configurationBuilder) =>
        //        {
        //            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath,"appsettings.json"), false, false).AddEnvironmentVariables();
        //            configuration = configurationBuilder.Build();
        //        })
        //        .ConfigureLogging((context, loggerBuilder) =>
        //        {
        //            loggerBuilder.AddConsole();
        //        })
        //        .ConfigureServices(services =>
        //        {
        //            services.RegisterFunctionAppServices(configuration);                    
        //        })
        //        .Build();


        //    return host.RunAsync();
        //}
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
                    configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "local.settings.json"), false, false);
                    configuration = configurationBuilder.Build();
                    var connectionString = configuration["ConnectionStrings:AppConfig"];
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