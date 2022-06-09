



namespace Beis.HelpToGrow.Voucher.FunctionApp.Extensions
{
    public static class RegisterFunctionAppServicesExtension
    {
        public static IServiceCollection RegisterFunctionAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddOptions();
            services.Configure<TokenReminderOptions>(options => configuration.Bind(options));
            services.Configure<EncryptionSettings>(options => configuration.Bind(options));
            services.Configure<EncryptionSettings>(options => configuration.Bind(options));
            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(configuration["HelpToGrowDbConnectionString"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();

            services.AddTransient<IEnterpriseRepository, EnterpriseRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddTransient<IEmailClientService, EmailClientService>();
            services.AddTransient<IVoucherGenerationService, VoucherGenerationService>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddScoped<INotifyServiceSettings, NotifyServiceSettings>();

            return services;
        }
    }
}