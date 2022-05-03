using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class ContactUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}