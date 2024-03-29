﻿
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class NotifyService : INotifyService
    {
        // notify dashboard - https://www.notifications.service.gov.uk/

        private readonly ILogger<NotifyService> _logger;
        private readonly VoucherWebAppNotifySettings _settings;
        private readonly IEmailClientService _client;
        private readonly IWebHostEnvironment _environment;

        public NotifyService(
            ILogger<NotifyService> logger,
            IOptions<VoucherWebAppNotifySettings> settings,
            IEmailClientService client,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _settings = settings.Value;
            _client = client;
            _environment = environment;
        }

        public async Task<Result> SendVerifyEmailNotification(ApplicantDto applicant, string templateId = null)
        {
            var verifyEmailTemplateId = templateId ?? _settings.VerifyApplicantEmailAddressTemplateId;
            var sendEmailAddress = applicant.EmailAddress; 
            
            try
            {
                var personalisation = new Dictionary<string, dynamic>
                {
                    {"email address", sendEmailAddress},
                    {"full name" , applicant.FullName},
                    {"work email address" , applicant.EmailAddress},
                    {"verification link", applicant.EmailVerificationLink },
                    {"subscribed", applicant.HasProvidedMarketingConsent ? "yes" : "no" },
                    {"unsubscribe link", applicant.HasProvidedMarketingConsent ? getUnsubscribeLink(applicant, _settings.EmailVerificationUrl) : string.Empty }

                };
                
                await _client.SendEmailAsync(applicant.EmailAddress, verifyEmailTemplateId, personalisation);
                
                return Result.Ok();
            }
            catch(NotifyClientException notifyException)
            {
                if(notifyException.Message.ToLower().Contains("teamid") && _environment.IsDevelopment()) // todo consider how to handle this
                {
                    _logger.LogError(notifyException, "Exception ignored in development (the notify service needs to be set to production settings");

                    return Result.Ok();
                }

                _logger.LogError(notifyException, "There was a problem sending the Verify Email Notification");
                    
                return Result.Fail(new Error($"There was a problem sending the Verify Email Notification :  {notifyException.Message}"));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "There was a problem sending the Verify Email Notification");

                return Result.Fail(new Error($"There was a problem sending the Verify Email Notification :  {ex.Message}"));
            }
        }

        private dynamic getCancelLink(ApplicantDto applicant)
        {
            var baseUri = new Uri(_settings.EmailVerificationUrl);

            var uriBuilder = new UriBuilder("https", baseUri.Host, baseUri.Port, "cancelvoucher", $"?enterpriseId={applicant.EnterpriseId}&emailAddress={applicant.EmailAddress}");

            return uriBuilder.ToString();
        }

        private static string getUnsubscribeLink(ApplicantDto applicant, string emailVerificationUrl)
        {
            var baseUri = new Uri(emailVerificationUrl);

            var uriBuilder = new UriBuilder("https", baseUri.Host, baseUri.Port, "unsubscribed", $"?enterpriseId={applicant.EnterpriseId}&emailAddress={applicant.EmailAddress}");

            return uriBuilder.ToString();
        }

        public async Task<Result> SendVoucherToApplicant(UserVoucherDto userVoucher)
        {
            if (string.IsNullOrWhiteSpace(userVoucher?.tokenPurchaseLink))
            {
                _logger.LogError("Unable to send the voucher link : The link is not valid");
                return Result.Fail("The link is not valid");
            }

            try
            {
                var personalisation = new Dictionary<string, dynamic>
                {
                    {"full name" , userVoucher.ApplicantDto.FullName},
                    {"voucher link" , userVoucher.tokenPurchaseLink},
                    {"product", userVoucher.SelectedProduct.product_name },
                    {"price", userVoucher.SelectedProduct.price },
                    {"cancel link", getCancelLink(userVoucher.ApplicantDto) }
                };

                await _client.SendEmailAsync(userVoucher.ApplicantDto.EmailAddress, _settings.IssueTokenTemplateId, personalisation);

                return Result.Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"There was a problem sending the voucher email: {ex.Message}");
                
                return Result.Fail($"There was a problem sending the voucher email: { ex.Message}");
            }
        }
    }
}