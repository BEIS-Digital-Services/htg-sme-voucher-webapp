
using Beis.HelpToGrow.Voucher.Web.Common;
using Beis.HelpToGrow.Voucher.Web.Config;
using Beis.HelpToGrow.Voucher.Web.Services;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;
using Beis.HelpToGrow.Voucher.Web.Services.FCAServices;
using Beis.HelpToGrow.Voucher.Web.Services.HealthCheck;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace Beis.HelpToGrow.Voucher.Web
{
    public class Startup
    {
        private const int SessionTimeOutMinutes = 30;
        private const int HstsMaxAgeDays = 365;

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddOptions();

            services.Configure<EligibilityRules>(_configuration.GetSection("EligibilityRules"));
            services.Configure<CookieNamesConfiguration>(_configuration.GetSection("CookieNamesConfiguration"));
            services.Configure<IndesserConnectionOptions>(options => _configuration.Bind(options));
            services.Configure<CompanyHouseHealthCheckConfiguration>(_configuration.GetSection("CompanyHouseHealthCheckConfiguration"));
            services.Configure<EncryptionSettings>(options => _configuration.Bind(options));
            services.Configure<UrlOptions>(o =>
            {
                o.EmailVerificationUrl = _configuration["EmailVerificationUrl"];
                o.LearningPlatformUrl = _configuration["LearningPlatformUrl"];
            });
            services.Configure<VoucherSettings>(_configuration.GetSection("VoucherSettings"));

            services.AddMvc();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews(config =>
            {
                config.Filters.Add<SharedResultFilter>();
                config.Filters.Add<SessionRequiredActionFilter>();
            });

            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);
            
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(HstsMaxAgeDays);
            });

            services.Configure<CookiePolicyOptions>(options => options.Secure = CookieSecurePolicy.Always);

            services.AddLogging(options => { options.AddConsole(); });
            services.AddApplicationInsightsTelemetry(_configuration["AzureMonitorInstrumentationKey"]);

            services.AddHttpContextAccessor();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddScoped<IFCASocietyService, FCASocietyService>();
            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<IEmailClientService, EmailClientService>();
            services.AddScoped<IEnterpriseService, EnterpriseService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IIndesserResponseService, IndesserResponseService>();
            services.AddScoped<IEligibilityCheckResultService, EligibilityCheckResultService>();
            services.AddScoped<ICompanyHouseResultService, CompanyHouseResultService>();
            services.AddScoped<IProductPriceService, ProductPriceService>();
            services.AddScoped<IApplicationStatusService, ApplicationStatusService>();
            services.AddScoped<IVoucherCancellationService, VoucherCancellationService>();

            services.AddSingleton<INotifyServiceSettings, NotifyServiceSettings>();
            services.AddSingleton<ICheckEligibility, EligibilityCheckService>();
            services.AddSingleton<IVerifyPostcodePattern, PostcodePattern>();
            services.AddSingleton<IVerifyMinTradingDuration, MinTradingDuration>();
            services.AddSingleton<IVerifyNonDissolution, ActivelyTrading>();
            services.AddSingleton<IVerifyEmployeeCount, EmployeeCount>();
            services.AddSingleton<IVerifyNoGazette, NoGazette>();
            services.AddSingleton<IVerifyDirectorNonDisqualification, DirectorDisqualification>();
            services.AddSingleton<IVerifyAccountFiling, AccountFiling>();
            services.AddSingleton<IVerifyNoAbnormalFiling, AbnormalFiling>();
            services.AddSingleton<IVerifyHoldingCompanyRegistration, HoldingCompanyRegistration>();
            services.AddSingleton<IVerifyRegisteredAddressUnchanged, RegisteredAddressUnchanged>();
            services.AddSingleton<IVerifySingleCompanyName, SingleCompanyName>();
            services.AddSingleton<IVerifyFinancialAgreementProviders, FinancialAgreementProviders>();
            services.AddSingleton<IVerifyTotalAgreements, TotalAgreements>();
            services.AddSingleton<IVerifyProtectFraudScore, ProtectFraudScore>();
            services.AddSingleton<IVerifyScoreCheck, ScoreCheck>();
            services.AddSingleton<IVerifyMortgagePresent, MortgagePresent>();

            services.AddSingleton<ICheckEligibilityRule, BR01>();
            services.AddSingleton<ICheckEligibilityRule, BR02>();
            services.AddSingleton<ICheckEligibilityRule, BR03>();
            services.AddSingleton<ICheckEligibilityRule, BR04>();
            services.AddSingleton<ICheckEligibilityRule, BR05>();
            services.AddSingleton<ICheckEligibilityRule, BR06>();
            services.AddSingleton<ICheckEligibilityRule, BR07>();
            services.AddSingleton<ICheckEligibilityRule, BR08>();
            services.AddSingleton<ICheckEligibilityRule, BR09>();
            services.AddSingleton<ICheckEligibilityRule, BR10>();
            services.AddSingleton<ICheckEligibilityRule, BR11>();
            services.AddSingleton<ICheckEligibilityRule, BR12>();
            services.AddSingleton<ICheckEligibilityRule, BR13>();
            services.AddSingleton<ICheckEligibilityRule, BR14>();
            services.AddSingleton<ICheckEligibilityRule, BR15>();
            services.AddSingleton<ICheckEligibilityRule, BR16>();

            services.AddHttpClient();

            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(_configuration["HelpToGrowDbConnectionString"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();

            services.AddSession(options =>
            {
                options.Cookie.Name = "smevoucherservice_session";
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(SessionTimeOutMinutes);                
            });
            
            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            var restClientFactory = new RestClientFactory();

            services.AddSingleton<IIndesserHttpConnection<IndesserCompanyResponse>, IndesserConnection>();

            services.AddSingleton <ICompanyHouseHttpConnection<CompanyHouseResponse>>(_ =>
                new CompanyHouseConnection(
                    restClientFactory,
                    _configuration["CompanyHouseUrl"],
                    _configuration["CompanyHouseApiKey"],
                    _configuration["VoucherSettings:connectionTimeOut"]));

            services.AddScoped<IFCASocietyRepository, FCASocietyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddScoped<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
            services.AddScoped<IEnterpriseSizeRepository, EnterpriseSizeRepository>();
            services.AddScoped<IIndesserResponseRepository, IndesserResponseRepository>();
            services.AddScoped<IEligibilityCheckResultRepository, EligibilityCheckResultRepository>();
            services.AddScoped<ICompanyHouseResultRepository, CompanyHouseResultRepository>();
            services.AddScoped<IProductPriceDescriptionRepository, ProductPriceDescriptionRepository>();
            services.AddScoped<IProductPriceRepository, ProductPriceRepository>();

            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddHealthChecks()
                .AddCheck<DependencyInjectionHealthCheckService>("Dependency Injection Health Checks")
                .AddCheck<IndesserHealthCheckService>("Indesser Service Health Checks")
                .AddCheck<CompanyHouseHealthCheckService>("Company House Api")
                .AddCheck<DatabaseHealthCheckService>("Database")
                .AddCheck<EncryptionHealthCheckService>("Encryption", failureStatus: HealthStatus.Unhealthy,
                   tags: new[] { "Encryption" });

            services.AddScoped<IVoucherGenerationService, VoucherGenerationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = WriteHealthResponse
                });
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }
        private static Task WriteHealthResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status",
                        healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description",
                        healthReportEntry.Value.Description);
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value,
                            item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(
                Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }
}