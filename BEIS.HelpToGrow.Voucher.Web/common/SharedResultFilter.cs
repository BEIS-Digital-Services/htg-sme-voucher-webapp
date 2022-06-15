

namespace BEIS.HelpToGrow.Voucher.Web.Common
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
        }

        private void SetupSatisfactionSurveyUrl(Controller controller)
        {
            controller.ViewData["SatisfactionSurveyUrl"] = Urls.GetSatisfactionSurveyUrl(_options.LearningPlatformUrl);
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