
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class ConfirmApplicantController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IFCASocietyService _fcaSocietyService;
        private readonly IProductPriceService _productPriceService;
        private readonly UrlOptions _urlOptions;

        public ConfirmApplicantController(
            ISessionService sessionService,
            IFCASocietyService fcaSocietyService,
            IProductPriceService productPriceService,
            IOptions<UrlOptions> urlOptions)
        {
            _sessionService = sessionService;
            _fcaSocietyService = fcaSocietyService;
            _productPriceService = productPriceService;
            _urlOptions = urlOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            
            if (!userVoucherDto.ApplicantDto.HasAcceptedTermsAndConditions || !userVoucherDto.ApplicantDto.HasAcceptedPrivacyPolicy || !userVoucherDto.ApplicantDto.HasAcceptedSubsidyControl)
            {
                return RedirectToAction("", "TermsAndConditions");
            }

            FCASociety society = null;

            if (userVoucherDto.HasFCANumber.ToBoolean() && !string.IsNullOrWhiteSpace(userVoucherDto.FCANumber))
            {
                society = await _fcaSocietyService.GetSociety(userVoucherDto.FCANumber);
            }

            if (userVoucherDto.SelectedProduct is null)
            {
                return RedirectToAction("Index", "SelectSoftware");
            }
         
            var concentByPhoneAndEmail = string.Empty;

            if (userVoucherDto.ApplicantDto.HasProvidedMarketingConsentByPhone && userVoucherDto.ApplicantDto.HasProvidedMarketingConsent)
            {
                concentByPhoneAndEmail = "Phone, Email";

            } else if (userVoucherDto.ApplicantDto.HasProvidedMarketingConsentByPhone)
            {
                concentByPhoneAndEmail = "Phone";

            } else if (userVoucherDto.ApplicantDto.HasProvidedMarketingConsent)
            {
                concentByPhoneAndEmail = "Email";
            }

            var viewModel = new ConfirmApplicantViewModel
            {
                FullName = userVoucherDto.ApplicantDto.FullName,
                Role = userVoucherDto.ApplicantDto.Role,
                EmailAddress = userVoucherDto.ApplicantDto.EmailAddress,
                PhoneNumber = userVoucherDto.ApplicantDto.PhoneNumber,
                SoftwareProduct = userVoucherDto.SelectedProduct?.product_name,
                CompanyName =  userVoucherDto.HasCompanyHouseNumber.ToBoolean() ? userVoucherDto.CompanyHouseResponse.CompanyName : society?.SocietyName,
                CompanyNumber = userVoucherDto.HasCompanyHouseNumber.ToBoolean() ? userVoucherDto.CompanyHouseResponse.CompanyNumber : society?.FullRegistrationNumber,
                HasAcceptedTermsAndConditions = userVoucherDto.ApplicantDto.HasAcceptedTermsAndConditions,
                HasAcceptedPrivacyPolicy = userVoucherDto.ApplicantDto.HasAcceptedPrivacyPolicy,
                HasAcceptedSubsidyControl = userVoucherDto.ApplicantDto.HasAcceptedSubsidyControl,
                HasProvidedMarketingConsent = userVoucherDto.ApplicantDto.HasProvidedMarketingConsent,
                ProductPrice = await _productPriceService.GetProductPrice(userVoucherDto.SelectedProduct.product_id),
                ComparisonToolURL = Urls.GetComparisonToolUrl(_urlOptions.LearningPlatformUrl),
                MarketingConsentResponse = concentByPhoneAndEmail
            };

            return View(viewModel);
        }
    }
}