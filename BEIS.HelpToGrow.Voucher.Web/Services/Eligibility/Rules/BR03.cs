
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR03 : CheckEligibilityRule
    {
        public override string Name => "Company is actively trading ";
        public override string Description => "The company is currently actively trading in the UK i.e., not dissolved.";

        private readonly ILogger<BR03> _logger;
        private readonly IVerifyNonDissolution _nonDissolution;

        public BR03(ILogger<BR03> logger, IVerifyNonDissolution nonDissolution)
        {
            _logger = logger;
            _nonDissolution = nonDissolution;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _nonDissolution.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, active trading status:{@ActiveTradingStatus}, result:{@Result}",
                nameof(BR03),
                indesserCompanyResponse.Identification?.companyNumber,
                _nonDissolution.ActiveTradingStatus,
                result);

            return result;
        }
    }
}