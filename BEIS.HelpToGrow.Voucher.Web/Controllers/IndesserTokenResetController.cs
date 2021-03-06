
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class IndesserTokenResetController : ControllerBase
    {
        private readonly IMemoryCache _cacheService;
        private readonly ILogger<IndesserTokenResetController> _logger;

        public IndesserTokenResetController(
            IMemoryCache cacheService,
            ILogger<IndesserTokenResetController> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Clearing Indesser connection token...");

                _cacheService.Remove("connectionToken");

                _logger.LogInformation("Cleared Indesser connection token...");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing Indesser connection token");

                return Problem();
            }
        }
    }
}