using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Models
{
    public class NotifyServiceSettings : INotifyServiceSettings
    {
        private readonly IConfiguration _configuration;

        public NotifyServiceSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string NotifyApiKey => _configuration["NOTIFY_API_KEY"];
        public string TokenRedeemEmailReminder1TemplateId => _configuration["NOTIFY_TOKEN_REDEEM_EMAIL_REMINDER_1_TEMPLATE_ID"];
        public string TokenRedeemEmailReminder2TemplateId => _configuration["NOTIFY_TOKEN_REDEEM_EMAIL_REMINDER_2_TEMPLATE_ID"];
        public string TokenRedeemEmailReminder3TemplateId => _configuration["NOTIFY_TOKEN_REDEEM_EMAIL_REMINDER_3_TEMPLATE_ID"];
        public string VerifyApplicantEmailAddressTemplateId => _configuration["NOTIFY_VERIFY_EMAIL_TEMPLATE_ID"];
        public string IssueTokenTemplateId => _configuration["NOTIFY_ISSUE_TOKEN_TEMPLATE_ID"];
    }
}