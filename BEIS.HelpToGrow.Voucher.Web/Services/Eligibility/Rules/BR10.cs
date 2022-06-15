
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR10 : CheckEligibilityRule
    {
        public override string Name => "Holding company registered outside of UK";
        public override string Description => "The holding company must NOT be registered outside of the UK.";
        private readonly ILogger<BR10> _logger;
        private readonly IVerifyHoldingCompanyRegistration _holdingCompanyRegistration;

        public BR10(ILogger<BR10> logger, IVerifyHoldingCompanyRegistration holdingCompanyRegistration)
        {
            _logger = logger;
            _holdingCompanyRegistration = holdingCompanyRegistration;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _holdingCompanyRegistration.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, non UK holding company registration status:{@Status}, result:{@Result}",
                nameof(BR10),
                indesserCompanyResponse.Identification?.companyNumber,
                _holdingCompanyRegistration.None,
                result);

            return result;
        }
    }
}