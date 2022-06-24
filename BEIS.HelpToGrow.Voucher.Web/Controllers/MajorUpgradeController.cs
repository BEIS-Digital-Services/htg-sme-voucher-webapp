
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class MajorUpgradeController: Controller
    {
        private readonly ISessionService _sessionService;

        public MajorUpgradeController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            var model = new MajorUpgradeViewModel
            {
                MajorUpgrade = userVoucherDto.MajorUpgrade,
                SelectedProduct = userVoucherDto.SelectedProduct
            };

            return View(model);
        }

        public IActionResult Forward(MajorUpgradeViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.MajorUpgrade)) // todo: work out why unit test doesn't correctly assess model state validity
            {
                ModelState.Clear();
                ModelState.AddModelError("MajorUpgrade", $"Select yes if this is a major software upgrade from a previous version of {userVoucherDto.SelectedProduct.product_name}");
                model.SelectedProduct = userVoucherDto.SelectedProduct;

                return View("Index", model);
            }

            userVoucherDto.MajorUpgrade = model.MajorUpgrade;

            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return userVoucherDto.MajorUpgrade.ToBoolean()
                ? RedirectToAction("Index", "CompanySize")
                : RedirectToAction("MajorUpgrade", "InEligible");
        }
    }
}