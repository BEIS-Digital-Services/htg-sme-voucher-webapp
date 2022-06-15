
namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class CompaniesHouseController : Controller
    {
        private readonly ISessionService _sessionService;
        public CompaniesHouseController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            var viewModel = new CompaniesHouseViewModel
            {
                HasCompaniesHouseNumber = userVoucherDto.HasCompanyHouseNumber,
                CompanySize = userVoucherDto.CompanySize
            };

            return View(viewModel);
        }

        public IActionResult GotANumber(CompaniesHouseViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (string.IsNullOrWhiteSpace(model.HasCompaniesHouseNumber))
            {
                ModelState.Clear();
                ModelState.AddModelError("HasCompaniesHouseNumber", "Select yes if the business has a Companies House number");
                return View("Index", model);
            }

            userVoucherDto.HasCompanyHouseNumber = model.HasCompaniesHouseNumber;

            if (userVoucherDto.HasCompanyHouseNumber.ToBoolean())
            {
                userVoucherDto.HasFCANumber = null;
                userVoucherDto.CompanyHouseNumber = null;
            }

            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return RedirectToAction("Index", userVoucherDto.HasCompanyHouseNumber.ToBoolean()
                ? "CompaniesHouseNumber"
                : "FCA");
        }
    }
}