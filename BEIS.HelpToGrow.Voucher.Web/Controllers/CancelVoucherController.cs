

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class CancelVoucherController : Controller
    {
        private readonly IVoucherCancellationService _voucherCancellationService;
        private readonly ILogger<CancelVoucherController> _logger;
        private readonly UrlOptions _urlOptions;

        public CancelVoucherController(IVoucherCancellationService voucherCancellationService, ILogger<CancelVoucherController> logger, IOptions<UrlOptions> urlOptions)
        {
            _voucherCancellationService = voucherCancellationService;
            _logger = logger;
            _urlOptions = urlOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long enterpriseId, string emailAddress)
        {
            var response = await _voucherCancellationService.CancelVoucherFromEmailLink(enterpriseId, emailAddress);
            _logger.LogInformation("The voucher cancellation link returned a rsponse of {response}", response);
            switch (response)
            {
                case CancellationResponse.SuccessfullyCancelled:
                case CancellationResponse.TokenExpired:
       
                {
                        return View(new Uri(_urlOptions.LearningPlatformUrl));                  
                }
                default:
                    {
                        return View("CantCancel");
                    }
            }
        }
    }
}