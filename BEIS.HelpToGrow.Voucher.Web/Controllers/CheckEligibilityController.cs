







using Newtonsoft.Json;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class CheckEligibilityController : Controller
    {
        private readonly IIndesserHttpConnection<IndesserCompanyResponse> _indesserHttpConnection;
        private readonly IIndesserResponseService _indesserResponseService;
        private readonly ICheckEligibility _eligibility;
        private readonly IEligibilityCheckResultService _eligibilityCheckResultService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly ILogger<CheckEligibilityController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IApplicationStatusService _applicationStatusService;

        public CheckEligibilityController(
            IIndesserHttpConnection<IndesserCompanyResponse> indesserHttpConnection,
            IIndesserResponseService indesserResponseService,
            ICheckEligibility eligibility,
            IEligibilityCheckResultService eligibilityCheckResultService,
            IEnterpriseService enterpriseService,
            ILogger<CheckEligibilityController> logger,
            ISessionService sessionService, 
            IApplicationStatusService applicationStatusService)
        {
            _indesserHttpConnection = indesserHttpConnection;
            _indesserResponseService = indesserResponseService;
            _eligibility = eligibility;
            _eligibilityCheckResultService = eligibilityCheckResultService;
            _enterpriseService = enterpriseService;
            _logger = logger;
            _sessionService = sessionService;
            _applicationStatusService = applicationStatusService;
        }

        public async Task<IActionResult> Index()
        {
            var sessionDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            var userVoucherDto = await _enterpriseService.GetUserVoucherFromEnterpriseAsync(sessionDto.ApplicantDto.EnterpriseId, sessionDto.SelectedProduct.product_id);
            
            if (string.IsNullOrWhiteSpace(userVoucherDto.SelectedProduct?.redemption_url))
            {
                throw new Exception($"The selected product redemption url does not exist for product {userVoucherDto.SelectedProduct?.product_description}");
            }


            var applicationStatus = await _applicationStatusService.GetApplicationStatus(userVoucherDto.CompanyHouseResponse?.CompanyNumber, userVoucherDto.FCANumber);
            switch (applicationStatus)
            {
                case ApplicationStatus.NewApplication:
                case ApplicationStatus.CancelledInFreeTrialCanReApply:
                case ApplicationStatus.CancelledNotRedeemedCanReApply:             
                case ApplicationStatus.CancelledCannotReApply:
                case ApplicationStatus.Ineligible:
                case ApplicationStatus.ActiveTokenRedeemed:
                case ApplicationStatus.TokenReconciled:                
                    {
                        return RedirectToAction("Index", "TokenNotIssued");
                    }
                case ApplicationStatus.EmailNotVerified:
                    {
                        return RedirectToAction("CheckEmailAddress", "ApplicantEmailAddress");
                    }
                case ApplicationStatus.EmailVerified:
                case ApplicationStatus.ActiveTokenNotRedeemed:
                case ApplicationStatus.TokenExpired:
                    {
                        //continue
                        break;
                    }                
            }

            if (!string.IsNullOrWhiteSpace(userVoucherDto.FCANumber))
            {
                await _enterpriseService.SetEligibilityStatusAsync(EligibilityStatus.Fca);
                return RedirectToAction("Index", "TokenIssued");
            }

            //check indesser enterprise record to see if indesser has already been called for this record.
            var eligibilityStatus = await _enterpriseService.GetEligibilityStatusAsync();
            if (eligibilityStatus is EligibilityStatus.Eligible or EligibilityStatus.ReviewRequired or EligibilityStatus.Fca)
            {
                _logger.LogInformation("Eligibility check already completed for {enterprise} with status {status}. proceeding to Token Issued.", eligibilityStatus, userVoucherDto.ApplicantDto?.EnterpriseId);
                return RedirectToAction("Index", "TokenIssued");
            }

            if (eligibilityStatus != EligibilityStatus.Unknown && eligibilityStatus != EligibilityStatus.Error)
            {
                _logger.LogInformation("Eligibility check already completed for {enterprise} with status {status}. proceeding to Token Not Issued.", eligibilityStatus, userVoucherDto.ApplicantDto?.EnterpriseId);
                return RedirectToAction("Index", "TokenNotIssued");
            }

            var indesserCallResult = RunIndesserCheck(userVoucherDto);

            if (indesserCallResult.IsFailed)
            {
                return RedirectToAction("IndesserUnavailable");
            }

            var indesserCallPersistence = await _indesserResponseService.SaveAsync(indesserCallResult.Value, userVoucherDto.ApplicantDto.EnterpriseId);

            if (indesserCallPersistence.IsFailed)
            {
                return RedirectToAction("Error", "Home");
            }


            var eligibilityCalculation = _eligibility.Check(userVoucherDto, indesserCallResult.Value);

            if (eligibilityCalculation.IsFailed)
            {
                return RedirectToAction("Error", "Home");
            }

            var eligibilityCheckResult = eligibilityCalculation.Value;

            await _enterpriseService.SetEligibilityStatusAsync(eligibilityCheckResult.Eligibility);

            await _eligibilityCheckResultService.SaveAsync(eligibilityCheckResult, indesserCallPersistence);

            return !eligibilityCheckResult.IsEligible
                ? RedirectToAction("Index", "TokenNotIssued")
                : RedirectToAction("Index", "TokenIssued");
        }

        public IActionResult IndesserUnavailable()
        {
            return View();
        }

        private Result<IndesserCompanyResponse> RunIndesserCheck(UserVoucherDto userVoucherDto)
        {
            var companiesHouseNumber = userVoucherDto.CompanyHouseResponse.CompanyNumber;

            var indesserCheckResult = _indesserHttpConnection.ProcessRequest(companiesHouseNumber, ControllerContext.HttpContext);

            if (indesserCheckResult.IsFailed)
            {
                _logger.LogWarning(JsonConvert.SerializeObject(indesserCheckResult.Errors));
            }

            return indesserCheckResult;
        }
    }
}