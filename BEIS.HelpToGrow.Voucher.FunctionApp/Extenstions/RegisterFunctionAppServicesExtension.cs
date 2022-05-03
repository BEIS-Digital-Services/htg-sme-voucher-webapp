using Beis.HelpToGrow.Core.Repositories;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Config;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using BEIS.HelpToGrow.Voucher.Web.Services.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.FunctionApp.Extenstions
{
    public static class RegisterFunctionAppServicesExtension
    {
        public static IServiceCollection RegisterFunctionAppServices(this IServiceCollection services, IConfiguration configuration )
        {
            services.AddHttpClient();
            services.AddOptions();
            services.Configure<EncryptionSettings>(options => configuration.Bind(options));
            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(configuration["HELPTOGROW_CONNECTIONSTRING"]));
            services.AddDataProtection().PersistKeysToDbContext<HtgVendorSmeDbContext>();

            services.AddTransient<INotifyServiceSettings, NotifyServiceSettings>();

            services.AddTransient<IEnterpriseRepository, EnterpriseRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IVendorCompanyRepository, VendorCompanyRepository>();


            services.AddTransient<IEmailClientService, EmailClientService>();
            services.AddTransient<IVoucherGenerationService, VoucherGenerationService>();

            services.AddTransient<IEncryptionService, EncryptionService>();
            return services;
        }
    }
}
