using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class UnsubscribedController : Controller
    {
        private readonly IEnterpriseService _enterpriseService;

        public UnsubscribedController(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long enterpriseId, string emailAddress)
        {
            await _enterpriseService.Unsubscribe(enterpriseId, emailAddress);

            return View();
        }
    }
}