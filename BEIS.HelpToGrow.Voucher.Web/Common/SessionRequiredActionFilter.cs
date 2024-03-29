﻿
namespace Beis.HelpToGrow.Voucher.Web.Common
{
    public class SessionRequiredActionFilter : IActionFilter
    {
        private readonly ISessionService _sessionService;

        public SessionRequiredActionFilter(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is HomeController 
                or SessionExpiredController 
                or VerifyEmailAddressController 
                or CancelVoucherController 
                or GuidanceController 
                or TermsAndConditionsController 
                or ContactUsController 
                or GetInTouchController)
            {
                return;
            }

            if (!_sessionService.HasValidSession(context.HttpContext) && context.Controller is Controller controller)
            {                
                context.Result = controller.RedirectToAction(String.Empty, "SessionExpired");              
            }
        }
    }
}