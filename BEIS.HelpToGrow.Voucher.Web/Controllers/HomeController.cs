using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Models;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ICookieService _cookieService;
        private readonly IProductRepository _productRepository;
        private readonly UrlOptions _urlOptions;
        private readonly ILogger<HomeController> _logger;
        private bool IsProductSelected { get; set; }

        public HomeController(
            ISessionService sessionService,
            ICookieService cookieService,
            IProductRepository productRepository,
            ILogger<HomeController> logger,
            IOptions<UrlOptions> urlOptions)
        {
            _sessionService = sessionService;
            _cookieService = cookieService;
            _productRepository = productRepository;
            _logger = logger;
            _urlOptions = urlOptions.Value;
        }

        public async Task<IActionResult> Index(ProductSelectionViewModel model)
        {
            var cookieBannerViewModel = _cookieService.SyncCookieSelection(ControllerContext.HttpContext.Request, new CookieBannerViewModel());
            var userVoucherDto = new UserVoucherDto();
            _sessionService.Remove("userVoucherDto", ControllerContext.HttpContext);
            userVoucherDto.CookieBannerViewModel = cookieBannerViewModel;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            if (userVoucherDto.SelectedProduct == null || userVoucherDto.SelectedProductType == null)
            {
                await SetProductFromQueryString(model.ProductId, model.ProductTypeId);

                if (!IsProductSelected)
                {
                    return RedirectToAction(nameof(SoftwareNotChosen));
                }
            }

            return View(new HomeViewModel 
            { 
                IsApplyForDiscountHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            });
        }

        private async Task SetProductFromQueryString(long productId, long productTypeId)
        {
            _logger.LogInformation("Selecting product from query string.");

            if (productId == 0 || productTypeId == 0)
            {
                IsProductSelected = false;
                return;
            }

            var productObj = await _productRepository.GetProductSingle(productId);
            var productTypeObj = (await _productRepository.ProductTypes()).SingleOrDefault(x => x.id == productTypeId);

            IsProductSelected =
                productTypeObj != null &&
                productObj != null &&
                productObj.product_type == productTypeId;

            if (IsProductSelected)
            {
                var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
                userVoucherDto.SelectedProduct = productObj;
                userVoucherDto.SelectedProductType = productTypeObj;
                _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
                _logger.LogInformation("The product was selected to {product}", userVoucherDto.SelectedProduct!.product_name);
            }
        }

        public IActionResult SoftwareNotChosen()
        {
            return View(new HomeViewModel { LearningPlatformUrl = _urlOptions.LearningPlatformUrl });
        }

        public IActionResult Cookies()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            return View(userVoucherDto.CookieBannerViewModel);
        }

        public async Task<IActionResult> ProcessCookie(string controllerName, string actionName, string cookieType, bool? isAccept)
        {
            var cookieProcessed = await _cookieService.ProcessCookie(cookieType, isAccept, ControllerContext.HttpContext.Response);
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (cookieType == "act")
            {
                userVoucherDto.CookieBannerViewModel.IsCookieProcessed = cookieProcessed;
                userVoucherDto.CookieBannerViewModel.GoogleAnalyticsCookieAccepted = isAccept.HasValue ? isAccept.Value ? "true" : "false" : "";
                userVoucherDto.CookieBannerViewModel.MarketingCookieAccepted = isAccept.HasValue ? isAccept.Value ? "true" : "false" : "";
                userVoucherDto.CookieBannerViewModel.IsAllCookieAccepted = isAccept.HasValue && isAccept.Value;
                userVoucherDto.CookieBannerViewModel.IsBannerClosed = true;

                _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

                if (controllerName.Equals("home", StringComparison.OrdinalIgnoreCase) && actionName.Equals("index", StringComparison.OrdinalIgnoreCase)
                    && userVoucherDto.SelectedProduct?.product_id  > 0 && userVoucherDto.SelectedProductType?.id > 0)
                {
                    return RedirectToAction(actionName, controllerName, new { userVoucherDto.SelectedProduct.product_id, product_type = userVoucherDto.SelectedProductType.id });
                }

                return RedirectToAction(actionName, controllerName);
            }

            if (cookieType == "close")
            {
                userVoucherDto.CookieBannerViewModel.IsBannerClosed = true;
            }

            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            if (controllerName.Equals("home", StringComparison.OrdinalIgnoreCase) && actionName.Equals("index", StringComparison.OrdinalIgnoreCase)
                && userVoucherDto.SelectedProduct?.product_id > 0 && userVoucherDto.SelectedProductType?.id > 0)
            {
                return RedirectToAction(actionName, controllerName, new { userVoucherDto.SelectedProduct.product_id, product_type = userVoucherDto.SelectedProductType.id });
            }

            return RedirectToAction(actionName, controllerName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCookiesPreferences(CookieBannerViewModel viewModel)
        {
            var cookieProcessed = await _cookieService.SaveCookiesPreferences(HttpContext);
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            viewModel.IsAllCookieAccepted = viewModel.IsGoogleAnalyticsCookieAccepted && viewModel.IsMarketingCookieAccepted;
            viewModel.IsCookieProcessed = cookieProcessed;
            viewModel.IsBannerClosed = true;//Not required yet.
            userVoucherDto.CookieBannerViewModel = viewModel;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            return RedirectToAction("Cookies");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier });
        }
    }
}