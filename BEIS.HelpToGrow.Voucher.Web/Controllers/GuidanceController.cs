
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class GuidanceController : Controller
    {
        private readonly UrlOptions _urlOptions;

        public GuidanceController(IOptions<UrlOptions> urlOptions)
        {
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new GuidanceViewModel
            {
                IsGeneralGuidanceHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            });
        }
    }
}