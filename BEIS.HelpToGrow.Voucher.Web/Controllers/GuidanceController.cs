using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class GuidanceController : Controller
    {
        public IActionResult Index()
        {
            return View(new GuidanceViewModel
            {
                IsGeneralGuidanceHidden = true
            });
        }
    }
}