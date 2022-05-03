using Beis.HelpToGrow.Core.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using BEIS.HelpToGrow.Voucher.Web.Models.Product;
using System.Linq;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class SelectProductTypeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IProductRepository _productRepository;

        public SelectProductTypeController(
            ISessionService sessionService,
            IProductRepository productRepository) 
        {
            _sessionService = sessionService;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new ProductTypeViewModel();

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext) ?? new UserVoucherDto();

            if (userVoucherDto.ProductTypeList == null)
            {
                userVoucherDto.ProductTypeList = await _productRepository.ProductTypes();

                _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            }

            viewModel.ProductTypeList = userVoucherDto.ProductTypeList;

            if (userVoucherDto.SelectedProductType != null)
            {
                viewModel.SelectedProductType = userVoucherDto.SelectedProductType;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProductTypeViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!ModelState.IsValid || model.ProductType < 1) // model validation doesn't work here (via unit test)
            {
                model.ProductTypeList = userVoucherDto.ProductTypeList;
                return View(model);
            }

            if (userVoucherDto.ProductTypeList == null || model.ProductType == 0)
            {
                return RedirectToAction("Index", "SelectSoftware");
            }

            userVoucherDto.SelectedProductType = userVoucherDto.ProductTypeList.FirstOrDefault(p => p.id == model.ProductType);

            var products = await _productRepository.GetProducts();

            var productList = products.Where(x => x.product_type == model.ProductType).ToList();

            userVoucherDto.ProductList = productList;

            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return RedirectToAction("Index", "SelectSoftware");
        }
    }
}