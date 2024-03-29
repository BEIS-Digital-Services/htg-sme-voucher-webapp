﻿
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR11 : CheckEligibilityRule
    {
        public override string Name => "Company registered address changes";
        public override string Description => "The company registered office address has changed within the last 3 months";
        private readonly ILogger<BR11> _logger;
        private readonly IVerifyRegisteredAddressUnchanged _registeredAddressUnchanged;

        public BR11(ILogger<BR11> logger, IVerifyRegisteredAddressUnchanged registeredAddressUnchanged)
        {
            _logger = logger;
            _registeredAddressUnchanged = registeredAddressUnchanged;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _registeredAddressUnchanged.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, registered office address has not changed statuses:{@Status}, result:{@Result}",
                nameof(BR11),
                indesserCompanyResponse.Identification?.companyNumber,
                string.Join(",", _registeredAddressUnchanged.Unchanged),
                result);

            return result;
        }
    }
}