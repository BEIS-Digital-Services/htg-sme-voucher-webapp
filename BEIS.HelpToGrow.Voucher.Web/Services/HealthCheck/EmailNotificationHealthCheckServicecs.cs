using Beis.Htg.VendorSme.Database.Models;
using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.HealthCheck
{
    public class EmailNotificationHealthCheckServicecs : IHealthCheck
    {
        private readonly ILogger<CompanyHouseHealthCheckService> _logger;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IOptions<CompanyHouseHealthCheckConfiguration> _companyHouseHealthCheckOptions;
        private readonly IOptions<UrlOptions> _options;

        public EmailNotificationHealthCheckServicecs(ILogger<CompanyHouseHealthCheckService> logger, IEmailVerificationService emailVerificationService, IOptions<CompanyHouseHealthCheckConfiguration> companyHouseHealthCheckOptions, IOptions<UrlOptions> options)
        {
            _logger = logger;
            _emailVerificationService = emailVerificationService;
            _companyHouseHealthCheckOptions = companyHouseHealthCheckOptions;
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = true;

            try
            {
                var userVoucherDto = new UserVoucherDto();
                userVoucherDto.ApplicantDto ??= new ApplicantDto();
                userVoucherDto.ApplicantDto.EmailAddress = "jawwad.baig@broadlight.io";
                userVoucherDto.ApplicantDto.FullName = "Jawwad Baig";
                userVoucherDto.ApplicantDto.EnterpriseId = 1;
                userVoucherDto.SelectedProduct ??= new product { product_id = 11 };                                  


            var verificationCode = _emailVerificationService.GetVerificationCode(userVoucherDto);

                userVoucherDto.ApplicantDto.EmailVerificationLink = GetVerificationLink(verificationCode, _options.Value.EmailVerificationUrl);

                var result = await _emailVerificationService.SendVerifyEmailNotificationAsync(userVoucherDto.ApplicantDto);

                if (result.IsFailed)
                {
                    isHealthy = false;
                }

            } catch (Exception ex)
            {
                isHealthy = false;
                _logger.LogError(ex, $"Notify health check failed. Its faild with {ex.Message}");
            }

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Notify health check failed.");
            }

            return
                new HealthCheckResult(
                    context.Registration.FailureStatus, "Notify health check failed.");
        }
        private static string GetVerificationLink(string verificationCode, string path)
        {
            var param = new Dictionary<string, string> { { "verificationCode", verificationCode } };

            return new Uri(QueryHelpers.AddQueryString(path, param)).ToString();
        }
    }
}
