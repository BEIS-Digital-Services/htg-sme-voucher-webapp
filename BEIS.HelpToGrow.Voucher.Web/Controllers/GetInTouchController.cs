using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class GetInTouchController : Controller
    {
        public IActionResult Index()
        {
            return View(new UsefulLinksViewModel 
            {  
                IsGetInTouchHidden = true
            });
        }
    }
}