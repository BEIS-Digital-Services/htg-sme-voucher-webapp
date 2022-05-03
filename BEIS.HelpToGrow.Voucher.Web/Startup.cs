using Beis.HelpToGrow.Core.Repositories;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using BEIS.HelpToGrow.Core.Repositories;
using BEIS.HelpToGrow.Core.Repositories.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using System;
using BEIS.HelpToGrow.Voucher.Web.Common;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;
using BEIS.HelpToGrow.Voucher.Web.Services.FCAServices;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using BEIS.HelpToGrow.Voucher.Web.Services.Models;
using IEncryptionService = BEIS.HelpToGrow.Voucher.Web.Services.Interfaces.IEncryptionService;
using BEIS.HelpToGrow.Voucher.Web.Services.Config;

namespace BEIS.HelpToGrow.Voucher.Web
{
    public class Startup
    {

        private const int SESSION_TIMEOUT_MINUTES = 30;
        private const int HSTS_MAX_AGE_DAYS = 365;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<EligibilityRules>(Configuration.GetSection("EligibilityRules"));
            services.Configure<CookieNamesConfiguration>(Configuration.GetSection("CookieNamesConfiguration"));
            services.Configure<IndesserConnectionOptions>(Configuration.GetSection("IndesserConnection"));
            services.Configure<EncryptionSettings>(options => Configuration.Bind(options));

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
                options.MaxAge = TimeSpan.FromDays(HSTS_MAX_AGE_DAYS);
            });

            services.Configure<CookiePolicyOptions>(options => options.Secure = CookieSecurePolicy.Always);

            services.AddLogging(options => { options.AddConsole(); });
            services.AddApplicationInsightsTelemetry(Configuration["AZURE_MONITOR_INSTRUMENTATION_KEY"]);

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

            services.AddSingleton(new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(SESSION_TIMEOUT_MINUTES)));
            
            services.AddHttpClient();

            services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration["REDIS_PRIMARY_CONNECTION_STRING"]; });

            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(Configuration["HELPTOGROW_CONNECTIONSTRING"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();

            services.AddSession(options =>
            {
                options.Cookie.Name = "smevoucherservice_session";
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(SESSION_TIMEOUT_MINUTES);                
            });

            services.AddSingleton<IDistributedCacheFactory, DistributedCacheFactory>();
            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            var restClientFactory = new RestClientFactory();

            services.AddSingleton<IIndesserHttpConnection<IndesserCompanyResponse>, IndesserConnection>();

            services.AddSingleton <ICompanyHouseHttpConnection<CompanyHouseResponse>>(_ =>
                new CompanyHouseConnection(
                    restClientFactory,
                    Configuration["COMPANY_HOUSE_URL"],
                    Configuration["COMPANY_HOUSE_API_KEY"],
                    Configuration["VoucherSettings:connectionTimeOut"]));

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
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }
    }
}