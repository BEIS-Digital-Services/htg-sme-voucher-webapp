using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR02 : CheckEligibilityRule
    {
        public int MinTradingDurationMonths = 12;
        public override string Name => $"Company has been trading over {MinTradingDurationMonths} months";
        public override string Description => "The company must have been incorporated for more than 12 months from today’s date";

        private readonly ILogger<BR02> _logger;
        private readonly IVerifyMinTradingDuration _minTradingDuration;

        public BR02(ILogger<BR02> logger, IVerifyMinTradingDuration minTradingDuration)
        {
            _logger = logger;
            _minTradingDuration = minTradingDuration;
        }
        
        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _minTradingDuration.Verify(indesserCompanyResponse, MinTradingDurationMonths);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, min trading duration:{@MinTradingDurationMonths}, result:{@Result}",
                nameof(BR02),
                indesserCompanyResponse.Identification?.companyNumber,
                MinTradingDurationMonths,
                result);

            return result;
        }
    }
}