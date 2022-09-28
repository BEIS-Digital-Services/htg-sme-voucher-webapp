
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class ApplicantEmailAddressController : Controller
    {
        private readonly UrlOptions _options;
        private readonly ISessionService _sessionService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IApplicationStatusService _applicationStatusService;

        public ApplicantEmailAddressController(
            ISessionService sessionService,
            IEmailVerificationService emailVerificationService, 
            IApplicationStatusService applicationStatusService,
            IOptions<UrlOptions> options)
        {
            _sessionService = sessionService;
            _emailVerificationService = emailVerificationService;
            _applicationStatusService = applicationStatusService;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            if (string.IsNullOrWhiteSpace(userVoucherDto?.ApplicantDto.EmailAddress))
            {
                return View(new EmailAddressViewModel());
            }

            var model = new EmailAddressViewModel
            {
                EmailAddress = userVoucherDto.ApplicantDto.EmailAddress
            };

            return View(model);
        }
        
        public async Task<IActionResult> CheckEmailAddress()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            var model = new EmailAddressViewModel();

            if (string.IsNullOrWhiteSpace(userVoucherDto?.ApplicantDto.EmailAddress))
            {                              
                return View(model);
            }

            model.EmailAddress = userVoucherDto?.ApplicantDto?.EmailAddress;
           

            var companyHouseNumber = userVoucherDto.CompanyHouseResponse?.CompanyNumber ?? userVoucherDto.CompanyHouseNumber;

            if (userVoucherDto.ApplicantDto.EnterpriseId == 0 && !await _emailVerificationService.CompanyNumberIsUnique(companyHouseNumber, userVoucherDto.FCANumber))
            {
                var applicationStatus = await _applicationStatusService.GetApplicationStatus(userVoucherDto.CompanyHouseResponse?.CompanyNumber, userVoucherDto.FCANumber);
                switch (applicationStatus)
                {
                    default:
                        {
                            // continue
                            break;
                        }
                    case ApplicationStatus.ActiveTokenNotRedeemed:
                    case ApplicationStatus.CancelledCannotReApply:
                    case ApplicationStatus.Ineligible:
                        {
                            return View("CompanyAlreadyExists", userVoucherDto);                            
                        }
                    case ApplicationStatus.EmailNotVerified:
                    case ApplicationStatus.EmailVerified:
                    case ApplicationStatus.ActiveTokenRedeemed:
                    case ApplicationStatus.TokenReconciled:
                   
                        {
                            return View("CompanyAlreadyExists", userVoucherDto);
                        }

                }
               
            }

            var saveResult = await _emailVerificationService.CreateOrUpdateEnterpriseDetailsAsync(userVoucherDto);

            if (saveResult.IsFailed)
            {
                return RedirectToAction("error", "home");
            }

            userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext); // todo - consider returning the dto from the service

            var verificationCode = _emailVerificationService.GetVerificationCode(userVoucherDto);

            userVoucherDto.ApplicantDto.EmailVerificationLink = GetVerificationLink(verificationCode, _options.EmailVerificationUrl);

            var result = await _emailVerificationService.SendVerifyEmailNotificationAsync(userVoucherDto.ApplicantDto);
            if(result.IsFailed)
            {
                return View("ServiceUnavailable");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Index(EmailAddressViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.EmailAddress)) // model state validity evaluated incorrectly in unit test!
            {
                return View("Index", model);
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext) ?? new UserVoucherDto();
            userVoucherDto.ApplicantDto ??= new ApplicantDto();
            userVoucherDto.ApplicantDto.EmailAddress = model.EmailAddress.Trim();
            _sessionService.Set("userVoucherDto", userVoucherDto, HttpContext);

            return RedirectToAction("Index", "ApplicantPhoneNumber");
        }

        private static string GetVerificationLink(string verificationCode, string path)
        {
            var param = new Dictionary<string, string> { { "verificationCode", verificationCode } };

            return new Uri(QueryHelpers.AddQueryString(path, param)).ToString();
        }
    }
}