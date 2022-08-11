using Microsoft.AspNetCore.Mvc;

namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class MarketingConsentController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<MarketingConsentController> _logger;

        public MarketingConsentController(ISessionService sessionService, ILogger<MarketingConsentController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new MarketingConsentViewModel();
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            if (userVoucherDto?.ApplicantDto == null )
            {
                return RedirectToAction("", "SessionExpired");
            }
            viewModel.AcceptMarketingByEmail = userVoucherDto.ApplicantDto.HasProvidedMarketingConsent;
            viewModel.AcceptMarketingByPhone = userVoucherDto.ApplicantDto.HasProvidedMarketingConsentByPhone;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MarketingConsentViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            if (userVoucherDto?.ApplicantDto == null)
            {
                _logger.LogError("Marketing consent was posted with no valid userVoucherDto in the session.");
                return RedirectToAction("", "SessionExpired");
            }
            userVoucherDto.ApplicantDto.HasProvidedMarketingConsent = model.AcceptMarketingByEmail;
            userVoucherDto.ApplicantDto.HasProvidedMarketingConsentByPhone = model.AcceptMarketingByPhone;
            return RedirectToAction("Index", "ConfirmApplicant");
        }

    }
}
