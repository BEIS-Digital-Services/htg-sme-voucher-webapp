
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class VerifyEmailAddressController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IEmailVerificationService _emailVerificationService;

        public VerifyEmailAddressController(
            ISessionService sessionService,
            IEmailVerificationService emailVerificationService)
        {
            _sessionService = sessionService;
            _emailVerificationService = emailVerificationService;
        }

        public async Task<IActionResult> Index(string verificationCode)
        {
            var result = await _emailVerificationService.VerifyEnterpriseFromCodeAsync(verificationCode);

            return View(result.IsSuccess
                ? "Success"
                : "InvalidCode");
        }

        public async Task<IActionResult> Success()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            if (userVoucherDto?.ApplicantDto?.EnterpriseId > 0 && userVoucherDto.SelectedProduct?.product_id > 0)
                userVoucherDto = await _emailVerificationService.GetUserVoucherFromEnterpriseAsync(userVoucherDto.ApplicantDto.EnterpriseId, userVoucherDto.SelectedProduct.product_id);
            return userVoucherDto?.ApplicantDto?.IsVerified ?? false
                ? View()
                : View("Error");
        }

        public async Task<IActionResult> ConfirmVerified()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);
            if (userVoucherDto?.ApplicantDto?.EnterpriseId > 0 && userVoucherDto.SelectedProduct?.product_id > 0)
                userVoucherDto = await _emailVerificationService.GetUserVoucherFromEnterpriseAsync(userVoucherDto.ApplicantDto.EnterpriseId, userVoucherDto.SelectedProduct.product_id);
            return View(userVoucherDto?.ApplicantDto?.IsVerified ?? false
                ? "Success"
                : "NotVerified");
        }
    }
}