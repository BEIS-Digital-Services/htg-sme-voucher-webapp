
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    [Route("api/eligibilityrules")]
    [ApiController]
    public class EligibilityRulesController : ControllerBase
    {
        private readonly IOptions<EligibilityRules> _eligibilityOptions;

        public EligibilityRulesController(IOptions<EligibilityRules> eligibilityOptions)
        {
            _eligibilityOptions = eligibilityOptions;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                _eligibilityOptions.Value.Core,
                _eligibilityOptions.Value.Additional
            });
        }
    }
}
