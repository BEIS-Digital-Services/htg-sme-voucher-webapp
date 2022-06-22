
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class GetInTouchController : Controller
    {
        private readonly UrlOptions _urlOptions;

        public GetInTouchController(IOptions<UrlOptions> urlOptions)
        {
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new UsefulLinksViewModel 
            {  
                IsGetInTouchHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            });
        }
    }
}