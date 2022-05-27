using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class StartupHealthCheckService : IHealthCheck
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceProvider _serviceProvider;

        public StartupHealthCheckService(IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
        {
            var isHealthy = true;
            await Task.FromResult(0);

            var all = Assembly.GetEntryAssembly()
                             .GetReferencedAssemblies()
                             .Select(Assembly.Load);

            var getListOfInterfaces = typeof(Startup).Assembly.ExportedTypes.Where(x => x.IsInterface).Select(x => x.Name);
            
            var getListOfClasses  = typeof(Startup).Assembly.ExportedTypes.Where(x => x.IsClass).Select(x => x.Name);
            var getListOfRepositoryInterfaces = typeof(Startup).Assembly.GetTypes().Where(x => x.IsInterface).Select(x => x.Name);

            var getListOfServices = new List<string>();
            var startups = _serviceProvider.GetServices<IStartup>().ToList();

            //Type eventType = typeof(eventCarrier.EventType);
            //Type processor = typeof(IEventProcessor<>);
            //Type generic = processor.MakeGenericType(eventType);
            var eventProcessor = _serviceProvider.GetService(typeof(object));

            //foreach (var service in _serviceProvider.GetServices(typeof())
            //{
            //    getListOfServices.Add(service.ToString());
            //}


            using (var scope = _serviceScopeFactory.CreateScope())
            {
                
                var eligibilityRules = scope.ServiceProvider.GetRequiredService<IOptions<EligibilityRules>>();
                var eligibilityRulesCoreRuleCount = eligibilityRules.Value.Core.Rules.Count;

                var enterpriseService = scope.ServiceProvider.GetRequiredService<IEnterpriseService>();
                var checkCompanyNumberIsUnique = await enterpriseService.CompanyNumberIsUnique("12345678");

                var enterpriseRepository = scope.ServiceProvider.GetRequiredService<IEnterpriseRepository>();
                var enterpriseByCompanyNumber = await enterpriseRepository.GetEnterpriseByCompanyNumber("12345678");

                dynamic result = scope.ServiceProvider.GetRequiredService<IOptions<EligibilityRules>>();
                result = scope.ServiceProvider.GetRequiredService<IEnterpriseRepository>();
                result = scope.ServiceProvider.GetRequiredService<IFCASocietyRepository>();
                result = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                result = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
                
                result = scope.ServiceProvider.GetRequiredService<IVendorCompanyRepository>();
                isHealthy = result is IVendorCompanyRepository;

                result = scope.ServiceProvider.GetRequiredService<IVendorCompanyRepository>();
                isHealthy = result is IVendorCompanyRepository;

            }

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Help to grown web app is healthy");
            }

            return new HealthCheckResult(
                    context.Registration.FailureStatus, "Help to grown web is unhealthy");
        }
    }
}
