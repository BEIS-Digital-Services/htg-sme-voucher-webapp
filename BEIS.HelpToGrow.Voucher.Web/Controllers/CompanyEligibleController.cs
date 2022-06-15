


namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class CompanyEligibleController: Controller
    {
        private readonly ISessionService _sessionService;

        public CompanyEligibleController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            
            return View(userVoucherDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier });
        }
    }
}