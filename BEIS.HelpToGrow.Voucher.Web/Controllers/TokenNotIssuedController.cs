
namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class TokenNotIssuedController : Controller
    {
        [HttpGet]        
        public IActionResult Index()
        {
            return View();
        }
    }
}