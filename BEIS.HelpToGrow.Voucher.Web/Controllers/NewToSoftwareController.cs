
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class NewToSoftwareController : Controller
    {
        private readonly ILogger<NewToSoftwareController> _logger;
        private readonly ISessionService _sessionService;
        private readonly UrlOptions _urlOptions;
        public NewToSoftwareController(ILogger<NewToSoftwareController> logger, ISessionService sessionService, IOptions<UrlOptions> urlOptions)
        {
            _logger = logger;
            _sessionService = sessionService;
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            if (userVoucherDto?.SelectedProduct == null)
            {
                _logger.LogInformation("selected product is null. redirecting to software not chosen page.");
                return RedirectToAction("SoftwareNotChosen", "Home");
            }

            var model = new NewToSoftwareViewModel
            {
                FirstTime = userVoucherDto.FirstTime,
                SelectedProduct = userVoucherDto.SelectedProduct,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            };

            return View(model);
        }

        public IActionResult Forward(NewToSoftwareViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.FirstTime))
            {
                ModelState.Clear();
                ModelState.AddModelError("FirstTime", $"Select yes if you are buying {userVoucherDto?.SelectedProduct?.product_name} for the first time");
                model.SelectedProduct = userVoucherDto?.SelectedProduct;
                model.LearningPlatformUrl = _urlOptions.LearningPlatformUrl;
                return View("Index", model);
            }

            userVoucherDto.FirstTime = model.FirstTime;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return userVoucherDto.FirstTime.Equals("yes", StringComparison.InvariantCultureIgnoreCase)
                ? RedirectToAction("Index", "ExistingCustomer")
                : RedirectToAction("NotFirstTime", "InEligible");
        }
    }
}