
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class ApplicantRoleController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<ApplicantRoleController> _logger;

        public ApplicantRoleController(ISessionService sessionService, ILogger<ApplicantRoleController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]

        public IActionResult Index()
        {
            try
            {
                var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

                if (string.IsNullOrWhiteSpace(userVoucherDto?.ApplicantDto.Role))
                {
                    return View(new TitleOrRoleViewModel());
                }

                var model = new TitleOrRoleViewModel
                {
                    BusinessRole = userVoucherDto.ApplicantDto.Role
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving applicant role page");

                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Index(TitleOrRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.BusinessRole))
                {
                    return View(nameof(Index), model);
                }

                var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext) ?? new UserVoucherDto();
                userVoucherDto.ApplicantDto ??= new ApplicantDto();
                userVoucherDto.ApplicantDto.Role = model.BusinessRole.Trim();
                _sessionService.Set("userVoucherDto", userVoucherDto, HttpContext);
                return RedirectToAction("Index", "ApplicantEmailAddress");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error progressing from applicant role page");

                return RedirectToAction("Error", "Home");
            }
        }
    }
}