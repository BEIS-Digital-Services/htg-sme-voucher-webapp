
namespace Beis.HelpToGrow.Voucher.FunctionApp
{
    public class TokenReminderFunction
    {
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;

        private readonly IEmailClientService _emailClientService;
        private readonly IVoucherGenerationService _voucherGenerationService;

        private readonly TokenReminderOptions _options;
        private readonly ILogger<TokenReminderFunction> _logger;
        private readonly IOptions<VoucherSettings> _voucherSettings;

        public TokenReminderFunction(IEnterpriseRepository enterpriseRepository,
            ITokenRepository tokenRepository,
            IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository,
            IEmailClientService emailClientService,
            IVoucherGenerationService voucherGenerationService,
            IOptions<TokenReminderOptions> options,
            ILogger<TokenReminderFunction> logger,
            IOptions<VoucherSettings> voucherSettings)
        {
            _enterpriseRepository = enterpriseRepository;
            _tokenRepository = tokenRepository;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;

            _emailClientService = emailClientService;
            _voucherGenerationService = voucherGenerationService;
            _options = options.Value;
            _logger = logger;
            _voucherSettings = voucherSettings;
        }

        [Function("TokenReminderFunction")]
        public async Task Run([Microsoft.Azure.Functions.Worker.TimerTrigger("%REMINDER_SCHEDULE%", RunOnStartup = false)] ScheduleInfo myTimer, FunctionContext context)
        {
            _logger.LogInformation($"Scheduled reminder email started processing for invocation: {context.InvocationId}.");

            await ProcessScheduledReminder(_options.TokenRedeemReminder1Days, true, false, false);

            await ProcessScheduledReminder(_options.TokenRedeemReminder2Days, false, true, false);

            await ProcessScheduledReminder(_options.TokenRedeemReminder3Days, false, false, true);
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
            var templateId = reminder1 ? _options.TokenRedeemEmailReminder1TemplateId
                                : reminder2 ? _options.TokenRedeemEmailReminder2TemplateId
                                            : _options.TokenRedeemEmailReminder3TemplateId;
            var eligibleProduct = await _productRepository.GetProductSingle(eligibleToken.product);
            _logger.LogInformation($"SendEmail fetched product info for product id: {eligibleToken.product}.");
            var vendor = await _vendorCompanyRepository.GetVendorCompanySingle(eligibleProduct.vendor_id);
            _logger.LogInformation($"SendEmail fetched vendor info for vendor id: {eligibleProduct.vendor_id}.");
            var voucherCode = await _voucherGenerationService.GenerateVoucher(vendor, eligibleEnterprise, eligibleProduct, _voucherSettings);
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