
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class TermsAndConditionsController: Controller
    {
        private readonly ISessionService _sessionService;
        private readonly UrlOptions _urlOptions;

        public TermsAndConditionsController(ISessionService sessionService, IOptions<UrlOptions> urlOptions)
        {
            _sessionService = sessionService;
            _urlOptions = urlOptions.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new TermsConditionsViewModel();
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            
            if (userVoucherDto == null)
            {
                return View(viewModel);
            }

            viewModel.SelectedProduct = userVoucherDto.SelectedProduct?.product_name;
            
            if (userVoucherDto.ConsentTermsConditions == null)
            {
                return View(viewModel);
            }

            viewModel.TermsAndConditions = true;
            viewModel.PrivacyPolicy = true;
            viewModel.SubsidyControl = true;


            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Terms()
        {
            var viewModel = new TermsConditionsViewModel 
            {
                IsTermsConditionsHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View( new UsefulLinksViewModel 
            {
                IsPrivacyPolicyHidden = true
            });
        }

        [HttpGet]
        public IActionResult Subsidy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TermsConditionsViewModel viewModel)
        {
            if (!ModelState.IsValid || viewModel.IsIncomplete)
            {
                ModelState.Clear();
                ModelState.AddModelError("TermsAndConditions", "Select if you have read and accepted the terms and conditions, privacy policy and subsidy control");
                return View(viewModel);
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            userVoucherDto.ConsentTermsConditions = "Yes";
            userVoucherDto.ApplicantDto.HasAcceptedTermsAndConditions = true;
            userVoucherDto.ApplicantDto.HasAcceptedPrivacyPolicy = true;
            userVoucherDto.ApplicantDto.HasAcceptedSubsidyControl = true;
       
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return RedirectToAction(String.Empty, "MarketingConsent");
        }
    }
}