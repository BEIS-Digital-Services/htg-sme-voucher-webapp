using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR13 : CheckEligibilityRule
    {
        public override string Name => "Total agreements";
        public override string Description => "The total number of the company’s financial agreements";
        private readonly ILogger<BR13> _logger;
        private readonly IVerifyTotalAgreements _totalAgreements;

        public BR13(ILogger<BR13> logger, IVerifyTotalAgreements totalAgreements)
        {
            _logger = logger;
            _totalAgreements = totalAgreements;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _totalAgreements.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, min total agreements:{@Status}, result:{@Result}",
                nameof(BR13),
                indesserCompanyResponse.Identification?.companyNumber,
                _totalAgreements.MinAgreementsCount,
                result);

            return result;
        }
    }
}