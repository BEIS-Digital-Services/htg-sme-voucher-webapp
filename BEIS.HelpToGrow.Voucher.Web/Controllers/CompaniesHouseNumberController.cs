using Microsoft.AspNetCore.Mvc;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using System.Net;
using BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;
using BEIS.HelpToGrow.Core.Enums;
using System.Linq;
using System.Collections.Generic;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class CompaniesHouseNumberController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ICompanyHouseHttpConnection<CompanyHouseResponse> _apiHttpConnection;
        private readonly ICompanyHouseResultService _companyHouseResultService;
        private readonly IVendorService _vendorService;
        private readonly IEnterpriseService _enterpriseService;
        private readonly IApplicationStatusService _applicationStatusService;
        public CompaniesHouseNumberController(
            ISessionService sessionService,
            ICompanyHouseHttpConnection<CompanyHouseResponse> apiHttpConnection,
            ICompanyHouseResultService companyHouseResultService,
            IVendorService vendorService,
            IEnterpriseService enterpriseService, IApplicationStatusService applicationStatusService)
        {
            _sessionService = sessionService;
            _apiHttpConnection = apiHttpConnection;
            _companyHouseResultService = companyHouseResultService;
            _vendorService = vendorService;
            _enterpriseService = enterpriseService;
            _applicationStatusService = applicationStatusService;
        }

        public IActionResult Index()
        {
            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            var viewModel = new CompaniesHouseViewModel
            {
                HasCompaniesHouseNumber = userVoucherDto.HasCompanyHouseNumber,
                CompanySize = userVoucherDto.CompanySize
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CheckCompanyDetails(CompaniesHouseNumberViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Number))
            {
                return View("CompaniesHouseNumber", model);
            }

            var userVoucherDto = _sessionService.Get<UserVoucherDto>("userVoucherDto", ControllerContext.HttpContext);

            if (!string.IsNullOrWhiteSpace(userVoucherDto?.FCANumber))
            {
                userVoucherDto.FCANumber = null;
                _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);
            }

            // check if the company is already in use before calling the api
            var enterpriseIsUnique = await _enterpriseService.CompanyNumberIsUnique(model.GetNumber());

            if (!enterpriseIsUnique)
            {
                var applicationStatus = await _applicationStatusService.GetApplicationStatus(model.GetNumber(), userVoucherDto.FCANumber);
                switch (applicationStatus)
                {
                    default:
                        {
                            // continue
                            break;
                        }
                    case ApplicationStatus.ActiveTokenNotRedeemed:
                    case ApplicationStatus.CancelledCannotReApply:
                    case ApplicationStatus.Ineligible:
                        {
                            return RedirectToAction("Vendor", "InEligible");
                        }
                    case ApplicationStatus.EmailNotVerified:
                    case ApplicationStatus.EmailVerified:
                    case ApplicationStatus.ActiveTokenRedeemed:
                    case ApplicationStatus.TokenReconciled:
                    
                        {
                            return View("CompanyAlreadyExists", userVoucherDto);

                       }

                }

            }

            var vendorFound = await _vendorService.IsRegisteredVendor(model.GetNumber());

            if (vendorFound)
            {
                return RedirectToAction("Vendor", "InEligible");
            }

            var companyHouseResponse = _apiHttpConnection.ProcessRequest(model.GetNumber(), ControllerContext.HttpContext);

            model.CompanyHouseResponse = companyHouseResponse;

            switch (companyHouseResponse.HttpStatusCode)
            {
                case HttpStatusCode.OK when companyHouseResponse.CompanyNumber == null:
                    {
                        return View("CompaniesHouseNumber", model);
                    }

                case HttpStatusCode.NotFound:
                    {
                        return View("CompaniesHouseNotFound");
                    }
            }

            if (companyHouseResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                return View("ServiceUnavailable");
            }

            userVoucherDto.CompanyHouseResponse = companyHouseResponse;

            _sessionService.Set("userVoucherDto", userVoucherDto, ControllerContext.HttpContext);

            await _companyHouseResultService.SaveAsync(companyHouseResponse);

            return View(model);
        }
    }
}