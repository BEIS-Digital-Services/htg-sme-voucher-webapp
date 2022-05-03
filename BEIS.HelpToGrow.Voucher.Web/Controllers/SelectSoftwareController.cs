using Beis.HelpToGrow.Core.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using BEIS.HelpToGrow.Voucher.Web.Models.Product;
using System.Linq;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class SelectSoftwareController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IProductRepository _productRepository;

        public SelectSoftwareController(ISessionService sessionService, IProductRepository productRepository)
        {
            _sessionService = sessionService;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new SelectSoftwareViewModel();

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if( userVoucherDto.ProductList == null || !userVoucherDto.ProductList.Any())
            {
                var products = await _productRepository.GetProducts();
                userVoucherDto.ProductList = products.Where(x => x.product_type == userVoucherDto.SelectedProductType.id).ToList();
                _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            }
            SetUpModelFromSession(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SelectSoftwareViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!ModelState.IsValid || model.ProductId == default)
            {
                ModelState.Clear();
                ModelState.AddModelError("ProductId", $"Select the {userVoucherDto?.SelectedProductType?.item_name} software you want to buy");
                SetUpModelFromSession(model);
                return View(model);
            }

            userVoucherDto.SelectedProduct = userVoucherDto.ProductList.First(x => x.product_id == model.ProductId);
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return RedirectToAction("Index", "ConfirmSoftware");
        } 

        private void SetUpModelFromSession(SelectSoftwareViewModel model)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            
            if (userVoucherDto == null)
            {
                return;
            }

            if(userVoucherDto.ProductList != null)
            {
                model.ProductList = userVoucherDto.ProductList;
            }

            if (userVoucherDto.SelectedProduct != null)
            {
                model.SelectedProduct = userVoucherDto.SelectedProduct;
            }

            if (userVoucherDto.SelectedProductType != null)
            {
                model.SelectedProductTypeName = userVoucherDto.SelectedProductType.item_name;
            }
        }
    }
}