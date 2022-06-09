using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class DependencyInjectionHealthCheckService : IHealthCheck
    {
        private readonly IControllerActivator _controllerActivator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DependencyInjectionHealthCheckService(IHttpContextAccessor httpContextAccessor, IControllerActivator controllerActivator)
        {
            _httpContextAccessor = httpContextAccessor;
            _controllerActivator = controllerActivator;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
        {
            var isHealthy = true;
            await Task.FromResult(0);
           
            var controllersList = GetChildTypes<ControllerBase>();

            string failedServiceErrorDetails = string.Empty;

            try
            {
                foreach (Type controller in controllersList)
                {
                    var controllerContext = new ControllerContext(new ActionContext(_httpContextAccessor.HttpContext,
                                                new RouteData(), new ControllerActionDescriptor
                                                {
                                                    ControllerTypeInfo = controller.GetTypeInfo()
                                                }));

                    var controllerInstance = _controllerActivator.Create(controllerContext);
                }
            } catch (Exception ex)
            {
                failedServiceErrorDetails = ex.Message;
                isHealthy = false;
            }

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Help to grow web app is healthy");
            }

            return new HealthCheckResult(
                    context.Registration.FailureStatus, $"Help to grow web is unhealthy; its failed with the following errors {failedServiceErrorDetails}");
        }
        private static IEnumerable<Type> GetChildTypes<T>()
        {
            var types = typeof(Startup).Assembly.GetTypes();
            return types.Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
        }
    }
}
