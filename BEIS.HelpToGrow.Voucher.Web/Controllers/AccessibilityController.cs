using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class AccessibilityController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}