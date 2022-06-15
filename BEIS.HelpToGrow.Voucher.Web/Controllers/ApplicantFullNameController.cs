

namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    [Route("applicant")] // this has been added to match the previous page link
    public class ApplicantFullNameController : Controller
    {
        private readonly ISessionService _sessionService;

        public ApplicantFullNameController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        
        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            if (string.IsNullOrWhiteSpace(userVoucherDto?.ApplicantDto.FullName))
            {
                return View();
            }

            var model = new FullNameViewModel
            {
                Name = userVoucherDto.ApplicantDto.FullName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Index(FullNameViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Name))
            {
                return View(nameof(Index), model);
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext) ?? new UserVoucherDto();
            userVoucherDto.ApplicantDto ??= new ApplicantDto();
            userVoucherDto.ApplicantDto.FullName = model.Name.Trim();
            _sessionService.Set("userVoucherDto", userVoucherDto, HttpContext);

            return RedirectToAction("Index", "ApplicantRole");
        }
    }
}