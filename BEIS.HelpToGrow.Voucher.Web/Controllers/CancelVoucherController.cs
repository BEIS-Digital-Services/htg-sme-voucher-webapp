using BEIS.HelpToGrow.Core.Enums;
using BEIS.HelpToGrow.Voucher.Web.Models;
using BEIS.HelpToGrow.Voucher.Web.Models.Applicant;

using BEIS.HelpToGrow.Voucher.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Controllers
{
    public class CancelVoucherController : Controller
    {
        private readonly IVoucherCancellationService _voucherCancellationService;
        private readonly ILogger<CancelVoucherController> _logger;

        public CancelVoucherController(IVoucherCancellationService voucherCancellationService, ILogger<CancelVoucherController> logger)
        {
            _voucherCancellationService = voucherCancellationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long enterpriseId, string emailAddress)
        {
            var response = await _voucherCancellationService.CancelVoucherFromEmailLink(enterpriseId, emailAddress);
            _logger.LogInformation("The voucher cancellation link returned a rsponse of {response}", response);
            switch (response)
            {
                case CancellationResponse.SuccessfullyCancelled:
                case CancellationResponse.TokenExpired:
       
                {
                        return View(new Uri(Urls.LearningPlatformUrl));                  
                }
                default:
                    {
                        return View("CantCancel");
                    }
            }

        }
    }
}