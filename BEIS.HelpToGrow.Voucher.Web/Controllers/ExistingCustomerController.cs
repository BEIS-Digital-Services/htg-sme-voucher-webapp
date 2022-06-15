
namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class ExistingCustomerController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;

        public ExistingCustomerController(ISessionService sessionService, 
            IProductRepository productRepository,
            IVendorCompanyRepository vendorCompanyRepository)
        {
            _sessionService = sessionService;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            var product = await _productRepository.GetProductSingle(userVoucherDto.SelectedProduct.product_id);
            var vendor = await _vendorCompanyRepository.GetVendorCompanySingle(product.vendor_id);
            userVoucherDto.VendorName = vendor.vendor_company_name;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            var model = new ExistingCustomerViewModel
            {
                ExistingCustomer = userVoucherDto.ExistingCustomer,
                VendorName = vendor.vendor_company_name
            };

            return View(model);
        }
        
        public IActionResult Forward(ExistingCustomerViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.ExistingCustomer))
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingCustomer", $"Select yes if you are an existing customer of { userVoucherDto.VendorName}");
                model.VendorName = userVoucherDto.VendorName;
                return View("Index", model);
            }

            userVoucherDto.ExistingCustomer = model.ExistingCustomer;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            
            return RedirectToAction("Index", userVoucherDto.ExistingCustomer.Equals("yes", StringComparison.InvariantCultureIgnoreCase)
                ? "MajorUpgrade"
                : "CompanySize");
        }
    }
}