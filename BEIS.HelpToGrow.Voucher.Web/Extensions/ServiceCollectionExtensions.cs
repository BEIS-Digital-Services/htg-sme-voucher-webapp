
namespace Beis.HelpToGrow.Voucher.Web.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private const int SessionTimeOutMinutes = 30;
        private const int HstsMaxAgeDays = 365;

        internal static void RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = "smevoucherservice_session";
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(SessionTimeOutMinutes);
            });
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(HstsMaxAgeDays);
            });
            services.AddLogging(options => { options.AddConsole(); });
            services.AddApplicationInsightsTelemetry(configuration["AzureMonitorInstrumentationKey"]);

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add<SharedResultFilter>();
                config.Filters.Add<SessionRequiredActionFilter>();
            });
            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(configuration["HelpToGrowDbConnectionString"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMemoryCache();
            services.AddOptions();
            services.AddHttpClient();

            services.AddSingleton(new DistributedCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromMinutes(SessionTimeOutMinutes)));
            services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisPrimaryConnectionString"]; });


            services.RegisterServices();
            services.RegisterOptions(configuration);

            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            
            services.AddSingleton<IIndesserHttpConnection<IndesserCompanyResponse>, IndesserConnection>();

            services.AddSingleton<ICompanyHouseHttpConnection<CompanyHouseResponse>, CompanyHouseConnection>();

            services.RegisterRepositories();

            services.AddSingleton<IEncryptionService, EncryptionService>();         

            services.AddScoped<IVoucherGenerationService, VoucherGenerationService>();

        }
        private static void RegisterServices(this IServiceCollection services)
        {
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
        }
        private static void RegisterRepositories(this IServiceCollection services)
        {
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
        }

        private static void RegisterOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EligibilityRules>(configuration.GetSection("EligibilityRules"));
            services.Configure<CookieNamesConfiguration>(configuration.GetSection("CookieNamesConfiguration"));
            services.Configure<IndesserConnectionOptions>(options => configuration.Bind(options));
            services.Configure<CompanyHouseHealthCheckConfiguration>(configuration.GetSection("CompanyHouseHealthCheckConfiguration"));
            
            services.Configure<CompanyHouseSettings>(options => configuration.Bind(options));

            services.Configure<EncryptionSettings>(options => configuration.Bind(options));
            services.Configure<UrlOptions>(o =>
            {
                o.EmailVerificationUrl = configuration["EmailVerificationUrl"];
                o.LearningPlatformUrl = configuration["LearningPlatformUrl"];
            });
            services.Configure<VoucherSettings>(configuration.GetSection("VoucherSettings"));
            services.Configure<NotifyServiceSettings>(options => configuration.Bind(options));
            services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.All);
            services.Configure<CookiePolicyOptions>(options => options.Secure = CookieSecurePolicy.Always);
        }

    }
}
