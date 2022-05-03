using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR16 : CheckEligibilityRule
    {
        public override string Name => "Mortgage Present";
        public override string Description => "The company has a current or satisfied mortgage present";
        private readonly ILogger<BR16> _logger;
        private readonly IVerifyMortgagePresent _mortgagePresent;

        public BR16(ILogger<BR16> logger, IVerifyMortgagePresent mortgagePresent)
        {
            _logger = logger;
            _mortgagePresent = mortgagePresent;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _mortgagePresent.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, mortgage check value:{@Status}, result:{@Result}",
                nameof(BR16),
                indesserCompanyResponse.Identification?.companyNumber,
                string.Join(",", _mortgagePresent.AMLR0006Value),
                result);

            return result;
        }
    }
}