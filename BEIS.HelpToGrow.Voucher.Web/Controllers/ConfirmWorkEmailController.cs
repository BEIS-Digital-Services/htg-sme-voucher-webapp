
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class ConfirmWorkEmailController: Controller
    {
        private readonly ISessionService _sessionService;

        public ConfirmWorkEmailController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index(string workEmail)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            
            userVoucherDto.WorkEmail = workEmail;

            _sessionService.Set("userVoucherDto", userVoucherDto, HttpContext);
            
            return View(userVoucherDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier });
        }
    }
}