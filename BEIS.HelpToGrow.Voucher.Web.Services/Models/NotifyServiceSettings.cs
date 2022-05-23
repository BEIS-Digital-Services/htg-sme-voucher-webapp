using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Models
{
    public class NotifyServiceSettings : INotifyServiceSettings
    {
        private readonly IConfiguration _configuration;

        public NotifyServiceSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string EmailVerificationUrl => _configuration["EmailVerificationUrl"];
        public string NotifyApiKey => _configuration["NotifyApiKey"];
        public string VerifyApplicantEmailAddressTemplateId => _configuration["NotifyVerifyEmailTemplateId"];
        public string IssueTokenTemplateId => _configuration["NotifyIssueTokenTemplateId"];
    }
}