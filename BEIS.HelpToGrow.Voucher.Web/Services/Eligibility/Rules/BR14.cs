
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR14 : CheckEligibilityRule
    {
        public override string Name => "Equifax Protect Fraud Score";
        public override string Description => "The protect score of a company. Those rated as slight increase in risk to severe risk must be referred for spot check (range between -200 to -900)";
        private readonly ILogger<BR14> _logger;
        private readonly IVerifyProtectFraudScore _protectFraudScore;

        public BR14(ILogger<BR14> logger, IVerifyProtectFraudScore protectFraudScore)
        {
            _logger = logger;
            _protectFraudScore = protectFraudScore;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _protectFraudScore.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, min protect score:{@Status}, result:{@Result}",
                nameof(BR14),
                indesserCompanyResponse.Identification?.companyNumber,
                _protectFraudScore.MinScoreAllowed,
                result);

            return result;
        }
    }
}