
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class TokenIssuedController: Controller
    {
        private readonly ILogger<TokenIssuedController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IVoucherGenerationService _voucherGenerationService;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly INotifyService _notifyService;
        private readonly IApplicationStatusService _applicationStatusService;
        private readonly IOptions<VoucherSettings> _voucherSettings;

        public TokenIssuedController(
            ILogger<TokenIssuedController> logger,
            ISessionService sessionService,
            IVoucherGenerationService voucherGenerationService,
            IVendorCompanyRepository vendorCompanyRepository,
            IEnterpriseRepository enterpriseRepository, INotifyService notifyService,
            IApplicationStatusService applicationStatusService, IOptions<VoucherSettings> voucherSettings)
        {
            _logger = logger;
            _sessionService = sessionService;
            _voucherGenerationService = voucherGenerationService;
            _vendorCompanyRepository = vendorCompanyRepository;
            _enterpriseRepository = enterpriseRepository;
            _notifyService = notifyService;
            _applicationStatusService = applicationStatusService;
            _voucherSettings = voucherSettings;
        }

        public async Task<IActionResult> Index()
        {

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);               

            _logger.LogInformation(
                "Getting token for enterprise {0}, product {1}, and url {2}",
                userVoucherDto.ApplicantDto?.EnterpriseId,
                userVoucherDto.SelectedProduct?.product_id,
                userVoucherDto.SelectedProduct?.redemption_url);

            var vendorCompany = await _vendorCompanyRepository.GetVendorCompanySingle(userVoucherDto.SelectedProduct?.vendor_id ?? -1);
            var eid = userVoucherDto.ApplicantDto?.EnterpriseId ?? throw new NullReferenceException($"Missing {nameof(userVoucherDto.ApplicantDto.EnterpriseId)}");
            var enterprise = await _enterpriseRepository.GetEnterprise(eid);

            var applicationStatus = await _applicationStatusService.GetApplicationStatus(userVoucherDto.CompanyHouseResponse?.CompanyNumber, userVoucherDto.FCANumber);
            switch (applicationStatus)
            {
                case ApplicationStatus.NewApplication:
                case ApplicationStatus.CancelledInFreeTrialCanReApply:
                case ApplicationStatus.CancelledNotRedeemedCanReApply:
                case ApplicationStatus.CancelledCannotReApply:
                    {
                        return RedirectToAction("CancelledCannotReApply", "InEligible");
                    }
                case ApplicationStatus.Ineligible:
                    {
                        return RedirectToAction("Ineligible", "InEligible");
                    }
                case ApplicationStatus.EmailVerified:
                case ApplicationStatus.ActiveTokenRedeemed:
                case ApplicationStatus.TokenReconciled:
                    {
                        return RedirectToAction("TokenReconciled", "InEligible");
                    }
                case ApplicationStatus.TokenExpired:
                    {
                        return View("CheckEligibility"); // todo - we should have better content pages
                    }
                case ApplicationStatus.EmailNotVerified:
                    {
                        _logger.LogError("There was an error issuing the token. The enterprise has not been verified.");
                        return RedirectToAction("EmailNotVerified", "InEligible");                        
                    }                             
                case ApplicationStatus.ActiveTokenNotRedeemed:
                    {
                        return RedirectToAction("ActiveTokenNotRedeemed", "InEligible");
                    }
                default:
                    {
                        //continue
                        break;
                    }
            }

            if (!new[]
            {
                (long)EligibilityStatus.Eligible,
                (long)EligibilityStatus.ReviewRequired,
                (long)EligibilityStatus.Fca
            }
            .Contains(enterprise.eligibility_status_id))
            {
                _logger.LogError("There was an error issuing the token. The enterprise has an eligibility status of {StatusCode}.", enterprise.eligibility_status_id);                 
                return View("CheckEligibility");
            }
            
            if (string.IsNullOrWhiteSpace(userVoucherDto.SelectedProduct?.redemption_url))
            {
                throw new Exception($"The selected product redemption url does not exist for product {userVoucherDto.SelectedProduct?.product_description}");
            }

            userVoucherDto.voucherCode = await _voucherGenerationService.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct, _voucherSettings);
            userVoucherDto.tokenPurchaseLink = GetTokenPurchaseLink(userVoucherDto, userVoucherDto.SelectedProduct);
            _logger.LogInformation("enterprise {id} has been generated a redemption url of {url}", enterprise.enterprise_id, userVoucherDto.tokenPurchaseLink);
            var notifyResult = await _notifyService.SendVoucherToApplicant(userVoucherDto);

            return notifyResult.IsFailed
                ? RedirectToAction(nameof(Error))
                : View(userVoucherDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier});
        }

        private string GetTokenPurchaseLink(UserVoucherDto userVoucherDto, product selectedProduct)
        {
            try
            {
                var param = new Dictionary<string, string> { { "grantToken", userVoucherDto.voucherCode.Trim() } };

                return new Uri(QueryHelpers.AddQueryString(selectedProduct.redemption_url.Trim(), param)).ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"There was an error generating the vendor token url for vendor website {selectedProduct.redemption_url}" +
                    $" and token {userVoucherDto.voucherCode}" +
                    $" : {ex.Message}");

                throw;
            }
        }
    }
}