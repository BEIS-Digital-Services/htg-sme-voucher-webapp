using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.FunctionApp
{
    public class TokenReminderFunction
    {
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;

        private readonly IEmailClientService _emailClientService;
        private readonly IVoucherGenerationService _voucherGenerationService;

        private readonly INotifyServiceSettings _settings;
        private readonly ILogger<TokenReminderFunction> _logger;
        private readonly IConfiguration _configuration;

        public TokenReminderFunction(IEnterpriseRepository enterpriseRepository,
            ITokenRepository tokenRepository,
            IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository,
            IEmailClientService emailClientService,
            IVoucherGenerationService voucherGenerationService,
            IConfiguration configuration,
            INotifyServiceSettings settings,
            ILogger<TokenReminderFunction> logger)
        {
            _enterpriseRepository = enterpriseRepository;
            _tokenRepository = tokenRepository;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;

            _emailClientService = emailClientService;
            _voucherGenerationService = voucherGenerationService;
            _configuration = configuration;
            _settings = settings;
            _logger = logger;
        }

        [Function("TokenReminderFunction")]
        public async Task Run([TimerTrigger("%REMINDER_SCHEDULE%",RunOnStartup = false)] TimerInfo myTimer, FunctionContext context)
        {
            _logger.LogInformation($"Scheduled reminder email started processing for invocation: {context.InvocationId}.");

            var reminder1Days = _configuration.GetValue<int>("ReminderConfig:Reminder1Days");
            await ProcessScheduledReminder(reminder1Days, true, false, false);

            var reminder2Days = _configuration.GetValue<int>("ReminderConfig:Reminder2Days");
            await ProcessScheduledReminder(reminder2Days, false, true, false);

            var reminder3Days = _configuration.GetValue<int>("ReminderConfig:Reminder3Days");
            await ProcessScheduledReminder(reminder3Days, false, false, true);
            _logger.LogInformation($"Scheduled reminder email processing completed successfully for invocation: {context.InvocationId}.");
        }

        private async Task ProcessScheduledReminder(int reminderDays, bool reminder1, bool reminder2, bool reminder3)
        {
            var eligibleTokens = await _tokenRepository.GetEnterpriseNotRedeemedToken(reminderDays, reminder1, reminder2, reminder3);
            _logger.LogInformation($"Eligible Token for {reminderDays} days is {eligibleTokens.Count()}.");
            
            foreach (var eligibleToken in eligibleTokens)
            {
                var eligibleEnterprise = await _enterpriseRepository.GetEnterprise(eligibleToken.enterprise_id);
                if (eligibleEnterprise == default(enterprise))
                {
                    _logger.LogInformation($"Missing EnterpriseId: {eligibleToken.enterprise_id}; this id was used in Token table.");
                }
                else
                {
                    _logger.LogInformation($"Enterprise id: {eligibleEnterprise.enterprise_id} was eligible for {reminderDays} days reminder on token valid from: {eligibleToken.token_valid_from} and expires on : {eligibleToken.token_expiry}.");
                    await SendEmail(eligibleToken, eligibleEnterprise, reminder1, reminder2, reminder3);
                    _logger.LogInformation($"Sent reminders to enterprise id: {eligibleEnterprise.enterprise_id} for {reminderDays} days reminder.");
                    await _tokenRepository.UpdateReminderStatus(eligibleToken.token_id, reminder1, reminder2, reminder3);
                    _logger.LogInformation($"Updated token email reminder sent status for token id: {eligibleToken.token_id}.");
                }
            }
        }

        private async Task SendEmail(token eligibleToken, enterprise eligibleEnterprise, bool reminder1, bool reminder2, bool reminder3)
        {
            var templateId = reminder1 ? _settings.TokenRedeemEmailReminder1TemplateId
                                : reminder2 ? _settings.TokenRedeemEmailReminder2TemplateId
                                            : _settings.TokenRedeemEmailReminder3TemplateId;
            var eligibleProduct = await _productRepository.GetProductSingle(eligibleToken.product);
            _logger.LogInformation($"SendEmail fetched product info for product id: {eligibleToken.product}.");
            var vendor = await _vendorCompanyRepository.GetVendorCompanySingle(eligibleProduct.vendor_id);
            _logger.LogInformation($"SendEmail fetched vendor info for vendor id: {eligibleProduct.vendor_id}.");
            var voucherCode = await _voucherGenerationService.GenerateVoucher(vendor, eligibleEnterprise, eligibleProduct);
            _logger.LogInformation($"SendEmail voucher code generated for vendor id: {eligibleProduct.vendor_id}.");
            var param = new Dictionary<string, string> { { "grantToken", voucherCode.Trim() } };

            var personalisation = new Dictionary<string, dynamic>
            {
                {"email address", eligibleEnterprise.applicant_email_address},
                {"full name", eligibleEnterprise.applicant_name}
            };

            if (!reminder3)
            {
                personalisation.Add("token expiry date", eligibleToken.token_expiry.ToString("dd MMMM yyyy"));
                personalisation.Add("token expiry time", eligibleToken.token_expiry.ToString("hh:mm:ss"));
                personalisation.Add("discount link", new Uri(QueryHelpers.AddQueryString(eligibleProduct.redemption_url.Trim(), param)).ToString());
            }

            await _emailClientService.SendEmailAsync(eligibleEnterprise.applicant_email_address, templateId, personalisation);
        }
    }
}