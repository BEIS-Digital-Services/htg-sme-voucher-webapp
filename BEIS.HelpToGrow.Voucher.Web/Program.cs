using Beis.HelpToGrow.Common.Helpers;
using Beis.HelpToGrow.Voucher.Web.Extensions;
using Beis.HelpToGrow.Voucher.Web.Services.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration(configBuilder =>
{
    var connectionString = configBuilder.Build().GetConnectionString("AppConfig");
    if (connectionString != null)
    {
        configBuilder.AddAzureAppConfiguration(connectionString);
    }
});

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Debug);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);

// Add services to the container.
builder.Services.RegisterAllServices(builder.Configuration);

builder.Services.AddHealthChecks()
              .AddCheck<DependencyInjectionHealthCheckService>("Dependency Injection Health Checks")
              .AddCheck<IndesserHealthCheckService>("Indesser Service Health Checks")
              .AddCheck<CompanyHouseHealthCheckService>("Company House Api")
              .AddCheck<DatabaseHealthCheckService>("Database")
              .AddCheck<EncryptionHealthCheckService>("Encryption", failureStatus: HealthStatus.Unhealthy,
                 tags: new[] { "Encryption" });

var app = builder.Build();
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseForwardedHeaders();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
    {
        ResponseWriter = HealthCheckJsonResponseWriter.Write
    });
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
});

app.Run();

