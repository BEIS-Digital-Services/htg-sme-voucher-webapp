using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR08 : CheckEligibilityRule
    {
        public override string Name => "Company has met account filing requirement";
        public override string Description => "Company must have met its accounts or annual return filing requirements.";
        private readonly ILogger<BR08> _logger;
        private readonly IVerifyAccountFiling _accountFiling;

        public BR08(ILogger<BR08> logger, IVerifyAccountFiling accountFiling)
        {
            _logger = logger;
            _accountFiling = accountFiling;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _accountFiling.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, account filing status:{@Status}, result:{@Result}",
                nameof(BR08),
                indesserCompanyResponse.Identification?.companyNumber,
                _accountFiling.None,
                result);

            return result;
        }
    }
}