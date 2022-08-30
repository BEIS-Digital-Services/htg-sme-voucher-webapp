using Microsoft.AspNetCore.Mvc;

namespace Beis.HelpToGrow.Voucher.Web.Controllers
{
    public class ApplicantPhoneNumberController : Controller
    {        
        private readonly ISessionService _sessionService;        

        public ApplicantPhoneNumberController(            
            ISessionService sessionService)
        {
            _sessionService = sessionService;            
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext);

            if (string.IsNullOrWhiteSpace(userVoucherDto?.ApplicantDto.PhoneNumber))
            {
                return View(new PhoneNumberViewModel());
            }

            var model = new PhoneNumberViewModel
            {
                PhoneNumber = userVoucherDto.ApplicantDto.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Index(PhoneNumberViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.PhoneNumber)) 
            {
                return View("Index", model);
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", HttpContext) ?? new UserVoucherDto();
            userVoucherDto.ApplicantDto ??= new ApplicantDto();
            userVoucherDto.ApplicantDto.PhoneNumber = model.PhoneNumber.Trim();
            _sessionService.Set("userVoucherDto", userVoucherDto, HttpContext);

            return RedirectToAction("Index", "TermsAndConditions");
        }   
    }
}
