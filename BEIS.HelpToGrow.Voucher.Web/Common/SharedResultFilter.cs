﻿
namespace Beis.HelpToGrow.Voucher.Web.Common
{
    public class SharedResultFilter : IResultFilter
    {
        private readonly ISessionService _sessionService;
        private readonly UrlOptions _options;

        public SharedResultFilter(ISessionService sessionService, IOptions<UrlOptions> options)
        {
            _sessionService = sessionService;
            _options = options.Value;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is not Controller controller)
            {
                return;
            }
            SetupCookieSelection(controller);
            SetupSatisfactionSurveyUrl(controller);
            SetupHelpToGrowDigitalUrl(controller);
            SetupGoogleAnalyticsKey(controller, context.Result as ViewResult);
        }

        private void SetupGoogleAnalyticsKey(Controller controller, ViewResult view)
        {
            var contentKey = controller.RouteData.Values["Controller"] + "-" + controller.RouteData.Values["Action"];
            if (!string.IsNullOrEmpty(view?.ViewName))
            {
                contentKey = contentKey + "-" + view.ViewName;
            }
            controller.ViewData["contentKey"] = contentKey.ToLower();
        }

        private void SetupSatisfactionSurveyUrl(Controller controller)
        {
            controller.ViewData["SatisfactionSurveyUrl"] = Urls.GetSatisfactionSurveyUrl(_options.LearningPlatformUrl);
        }
        private void SetupHelpToGrowDigitalUrl(Controller controller)
        {
            controller.ViewData["ComparisonToolUrl"] = Urls.GetComparisonToolUrl(_options.LearningPlatformUrl);
            controller.ViewData["ComparisonToolNoJsUrl"] = Urls.GetComparisonToolNoJsUrl(_options.LearningPlatformUrl);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        private void SetupCookieSelection(Controller controller)
        {
            var actionName = controller.RouteData.Values["action"]?.ToString();
            var controllerName = controller.RouteData.Values["controller"]?.ToString();
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", controller.HttpContext) ?? new UserVoucherDto { CookieBannerViewModel = new CookieBannerViewModel()};
            
            userVoucherDto.CookieBannerViewModel ??= new CookieBannerViewModel();
            userVoucherDto.CookieBannerViewModel.ActionName = actionName;
            userVoucherDto.CookieBannerViewModel.ControllerName = controllerName;
            userVoucherDto.CookieBannerViewModel.PageName = actionName;

            if (userVoucherDto.SelectedProduct?.product_id > 0 && userVoucherDto.SelectedProductType?.id > 0)
            {
                userVoucherDto.CookieBannerViewModel.ProductId = userVoucherDto.SelectedProduct.product_id;
                userVoucherDto.CookieBannerViewModel.ProductType = userVoucherDto.SelectedProductType.id;
            }
            
            _sessionService.Set("userVoucherDto", userVoucherDto, controller.HttpContext);
            controller.ViewData["CookieBannerViewModel"] = userVoucherDto.CookieBannerViewModel;
        }
    }
}