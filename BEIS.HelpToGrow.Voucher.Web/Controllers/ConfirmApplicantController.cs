using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BEIS.HelpToGrow.Voucher.Web.Models.Applicant;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions;
using BEIS.HelpToGrow.Voucher.Web.Services.FCAServices;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class ConfirmApplicantController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IFCASocietyService _fcaSocietyService;
        private readonly IProductPriceService _productPriceService;

        public ConfirmApplicantController(
            ISessionService sessionService,
            IFCASocietyService fcaSocietyService,
            IProductPriceService productPriceService)
        {
            _sessionService = sessionService;
            _fcaSocietyService = fcaSocietyService;
            _productPriceService = productPriceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            
            if (!userVoucherDto.ApplicantDto.HasAcceptedTermsAndConditions || !userVoucherDto.ApplicantDto.HasAcceptedPrivacyPolicy || !userVoucherDto.ApplicantDto.HasAcceptedSubsidyControl)
            {
                return RedirectToAction("", "TermsAndConditions");
            }

            FCASociety society = null;

            if (userVoucherDto.HasFCANumber.ToBoolean() && !string.IsNullOrWhiteSpace(userVoucherDto.FCANumber))
            {
                society = await _fcaSocietyService.GetSociety(userVoucherDto.FCANumber);
            }

            if (userVoucherDto.SelectedProduct is null)
            {
                return RedirectToAction("Index", "SelectSoftware");
            }

            var viewModel = new ConfirmApplicantViewModel
            {
                FullName = userVoucherDto.ApplicantDto.FullName,
                Role = userVoucherDto.ApplicantDto.Role,
                EmailAddress = userVoucherDto.ApplicantDto.EmailAddress,
                SoftwareProduct = userVoucherDto.SelectedProduct?.product_name,
                CompanyName =  userVoucherDto.HasCompanyHouseNumber.ToBoolean() ? userVoucherDto.CompanyHouseResponse.CompanyName : society?.SocietyName,
                CompanyNumber = userVoucherDto.HasCompanyHouseNumber.ToBoolean() ? userVoucherDto.CompanyHouseResponse.CompanyNumber : society?.FullRegistrationNumber,
                HasAcceptedTermsAndConditions = userVoucherDto.ApplicantDto.HasAcceptedTermsAndConditions,
                HasAcceptedPrivacyPolicy = userVoucherDto.ApplicantDto.HasAcceptedPrivacyPolicy,
                HasAcceptedSubsidyControl = userVoucherDto.ApplicantDto.HasAcceptedSubsidyControl,
                HasProvidedMarketingConsent = userVoucherDto.ApplicantDto.HasProvidedMarketingConsent,
                ProductPrice = await _productPriceService.GetProductPrice(userVoucherDto.SelectedProduct.product_id),
                ComparisonToolURL = Urls.ComparisonToolUrl
            };

            return View(viewModel);
        }
    }
}