using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR05 : CheckEligibilityRule
    {
        public override string Name => "No gazette data present on the company";
        public override string Description => "The company does not have any presence of Gazette data as sourced from various advertised and unadvertised data sources e.g., insolvency";

        private readonly ILogger<BR05> _logger;
        private readonly IVerifyNoGazette _noGazette;

        public BR05(ILogger<BR05> logger, IVerifyNoGazette noGazette)
        {
            _logger = logger;
            _noGazette = noGazette;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _noGazette.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, expected gazette status:{@Status}, result:{@Result}",
                nameof(BR05),
                indesserCompanyResponse.Identification?.companyNumber,
                _noGazette.None,
                result);

            return result;
        }
    }
}