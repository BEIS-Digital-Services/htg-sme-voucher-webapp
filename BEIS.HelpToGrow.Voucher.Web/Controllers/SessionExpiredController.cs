
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class SessionExpiredController : Controller
    {
        private readonly UrlOptions _urlOptions;

        public SessionExpiredController(IOptions<UrlOptions> urlOptions)
        {
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new SessionExpiredViewModel { LearningPlatformUrl = _urlOptions.LearningPlatformUrl });
        }
    }
}