
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class CompanySizeController: Controller
    {
        private readonly ISessionService _sessionService;

        public CompanySizeController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new CompanySizeViewModel();
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            if (userVoucherDto.EmployeeNumbers != default)
            {
                model.EmployeeNumbers = userVoucherDto.EmployeeNumbers;
            }

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(CompanySizeViewModel model)
        {
            if (!ModelState.IsValid || model.EmployeeNumbers == default)
            {
                ModelState.Clear();
                ModelState.AddModelError("EmployeeNumbers", "Enter the number of employees");
                return View(model);
            }

            if (model.EmployeeNumbers is < 1 or > 249)
            {
                return RedirectToAction("CompanySize", "InEligible");
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            userVoucherDto.EmployeeNumbers = model.EmployeeNumbers;
            userVoucherDto.CompanySize = "Yes";
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            return RedirectToAction("Index", "CompaniesHouse");
        }

        public IActionResult Back()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            return userVoucherDto.ExistingCustomer.ToBoolean() ? 
                RedirectToAction("Index", "MajorUpgrade")
                : RedirectToAction("Index", "ExistingCustomer");
        }
    }
}