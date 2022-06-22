
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class WorkEmailController: Controller
    {
        private readonly ISessionService _sessionService;

        public WorkEmailController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            return View(userVoucherDto);
        }
    }
}