using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class GuidanceController : Controller
    {
        private readonly UrlOptions _urlOptions;

        public GuidanceController(IOptions<UrlOptions> urlOptions)
        {
            _urlOptions = urlOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new GuidanceViewModel
            {
                IsGeneralGuidanceHidden = true,
                LearningPlatformUrl = _urlOptions.LearningPlatformUrl
            });
        }
    }
}