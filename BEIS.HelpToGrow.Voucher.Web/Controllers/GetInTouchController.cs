using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class GetInTouchController : Controller
    {
        private readonly UrlOptions _urlOptions;

        public GetInTouchController(IOptions<UrlOptions> urlOptions)
        {
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new UsefulLinksViewModel 
            {  
                IsGetInTouchHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            });
        }
    }
}