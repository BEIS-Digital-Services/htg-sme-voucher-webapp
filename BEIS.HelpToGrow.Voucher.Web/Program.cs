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

builder.Services.RegisterHealthcheckServices();

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
    endpoints.MapSMEHealthChecks();
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
});

app.Run();

