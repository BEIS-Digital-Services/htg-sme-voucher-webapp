using Beis.HelpToGrow.Core.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using BEIS.HelpToGrow.Voucher.Web.Models.Applicant;
using System;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.FCA;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.FCAServices;
using BEIS.HelpToGrow.Core.Enums;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class FCAController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IFCASocietyRepository _fcaSocietyRepository;
        private readonly IFCASocietyService _fcaSocietyService;
        private readonly IApplicationStatusService _applicationStatusService;

        public FCAController(ISessionService sessionService,
                            IEnterpriseRepository enterpriseRepository,
                            IFCASocietyRepository fcaSocietyRepository,
                            IFCASocietyService fcaSocietyService,
                            IApplicationStatusService applicationStatusService)
        {
            _sessionService = sessionService;
            _enterpriseRepository = enterpriseRepository;
            _fcaSocietyRepository = fcaSocietyRepository;
            _fcaSocietyService = fcaSocietyService;
            _applicationStatusService = applicationStatusService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            var viewModel = new FCANumberViewModel();
            if (userVoucherDto != null)
            {
                viewModel.HasFCANumber = userVoucherDto.HasFCANumber;
            }

            return View(viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Forward(FCANumberViewModel viewModel)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            if(!ModelState.IsValid || string.IsNullOrWhiteSpace(viewModel.HasFCANumber))
            {
                return View("Index", viewModel);
            }

            userVoucherDto.HasFCANumber = viewModel.HasFCANumber;
               
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            if (userVoucherDto.HasFCANumber?.ToLower() == "yes")
            {
                userVoucherDto.HasCompanyHouseNumber = "No";
                userVoucherDto.CompanyHouseNumber = null;
                userVoucherDto.CompanyHouseResponse = null;
                return RedirectToAction("GetFCANumber", "FCA");
            }
            
            return RedirectToAction("FCA", "Ineligible");
        }
        
        [HttpGet]
        public IActionResult GetFCANumber()
        {
            return View(new FCAViewModel());
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> GetFCANumber(FCAViewModel viewModel)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(viewModel.GetRegistrationNumber())) // todo: have unit test use model state validation
            {
                ModelState.Clear();
                ModelState.AddModelError("FCAFullRegistrationNumber", "Enter the Financial Conduct Authority Mutuals Public Register number");
                return View(viewModel);
            }

            var fcaNumberInEnterprise = await _enterpriseRepository.GetEnterpriseByFCANumber(viewModel.GetRegistrationNumber());

            if (fcaNumberInEnterprise != null)
            {
                StoreFCARegistrationNumberInSession(viewModel.GetRegistrationNumber());

                var applicationStatus = await _applicationStatusService.GetApplicationStatusForFcaNumber(viewModel.GetRegistrationNumber());
                switch (applicationStatus)
                {
                    case ApplicationStatus.NewApplication:
                    case ApplicationStatus.CancelledInFreeTrialCanReApply:
                    case ApplicationStatus.CancelledNotRedeemedCanReApply:
                        {
                            // continue
                            break;
                        }
                    case ApplicationStatus.ActiveTokenNotRedeemed:
                    case ApplicationStatus.CancelledCannotReApply:
                    case ApplicationStatus.Ineligible:
                        {
                            // return RedirectToAction("Deregistered", "InEligible"); // todo - we should have a more suitable content page
                            return RedirectToAction("VoucherAlreadyApplied", "FCA");
                        }
                    case ApplicationStatus.EmailNotVerified:
                    case ApplicationStatus.EmailVerified:
                    case ApplicationStatus.ActiveTokenRedeemed:
                    case ApplicationStatus.TokenReconciled:
                    case ApplicationStatus.TokenExpired:
                        {
                 
                            return RedirectToAction("VoucherAlreadyApplied", "FCA");
                       }
                }
            }

            //TODO: This should be one off ideally run from a stand alone process before fetching from the repo.
            if (await _fcaSocietyRepository.GetFCASocietiesCount() == 0)
            {
                await _fcaSocietyService.LoadFCASocieties();
            }

            var fcaSociety = await _fcaSocietyRepository.GetFCASociety(viewModel.GetRegistrationNumber());

            if(fcaSociety == null)
            {
                return RedirectToAction("FCANumberNotFound", "FCA");
            }

            if(fcaSociety.society_status.Equals("deregistered", StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction("Deregistered", "InEligible");
            }

            StoreFCARegistrationNumberInSession(viewModel.GetRegistrationNumber());
            
            return RedirectToAction("CheckCompanyDetails", "FCA");
        }

        public IActionResult VoucherAlreadyApplied()
        {
            return View();
        }

        public async Task<IActionResult> CheckCompanyDetails()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            var model = await _fcaSocietyService.GetSociety(userVoucherDto.FCANumber);
            return View(model);
        }

        public IActionResult FCANumberNotFound()
        {
            return View();
        }

        private void StoreFCARegistrationNumberInSession(string fcaNumber)
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);
            userVoucherDto.FCANumber = fcaNumber;
            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
        }
    }
}