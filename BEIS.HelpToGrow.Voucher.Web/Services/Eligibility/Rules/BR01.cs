using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR01 : CheckEligibilityRule
    {
        public override string Name => "Company address is registered in UK";
        public override string Description => "The registered office post code for limited companies and primary trading address for non-limited businesses is in the UK";

        private readonly ILogger<BR01> _logger;
        private readonly IVerifyPostcodePattern _postcodePattern;

        public BR01(ILogger<BR01> logger, IVerifyPostcodePattern postcodePattern)
        {
            _logger = logger;
            _postcodePattern = postcodePattern;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _postcodePattern.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, postcode:{@Postcode}, result:{@Result}", 
                nameof(BR01),
                indesserCompanyResponse.Identification?.companyNumber,
                indesserCompanyResponse.Identification?.RegisteredOffice?.postcode,
                result);
            
            return result;
        }
    }
}