
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class InEligibleController : Controller
    {
        private readonly ISessionService _sessionService;

        public InEligibleController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult MajorUpgrade()
        {
            return View(_sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext));
        }

        public IActionResult NotFirstTime()
        {
            return View(_sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext));
        }

        public IActionResult Vendor()
        {
            return View(_sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext));
        }

        public IActionResult Deregistered()
        {
            return View(_sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext));
        }

        public IActionResult CompanySize()
        {
            return View();
        }

        public IActionResult FCA()
        {
            return View();
        }
    }
}