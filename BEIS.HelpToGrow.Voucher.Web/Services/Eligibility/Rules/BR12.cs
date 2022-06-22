
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR12 : CheckEligibilityRule
    {
        public override string Name => "Company registered address changes";
        public override string Description => "The company registered office address has changed within the last 3 months";
        private readonly ILogger<BR12> _logger;
        private readonly IVerifySingleCompanyName _singleCompanyName;

        public BR12(ILogger<BR12> logger, IVerifySingleCompanyName singleCompanyName)
        {
            _logger = logger;
            _singleCompanyName = singleCompanyName;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _singleCompanyName.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, single company match status:{@Status}, result:{@Result}",
                nameof(BR12),
                indesserCompanyResponse.Identification?.companyNumber,
                _singleCompanyName.None,
                result);

            return result;
        }
    }
}