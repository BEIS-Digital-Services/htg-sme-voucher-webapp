
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR15 : CheckEligibilityRule
    {
        public override string Name => "Scorecheck Score";
        public override string Description => "The Scorecheck score associated with the immediate holding company. The following codes will flag a concern and will result in an indicator to refer to spot check: G, I, O, N, NA, NR, NT";
        private readonly ILogger<BR15> _logger;
        private readonly IVerifyScoreCheck _scoreCheck;

        public BR15(ILogger<BR15> logger, IVerifyScoreCheck scoreCheckCheck)
        {
            _logger = logger;
            _scoreCheck = scoreCheckCheck;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _scoreCheck.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, ineligible score codes:{@Status}, result:{@Result}",
                nameof(BR15),
                indesserCompanyResponse.Identification?.companyNumber,
                string.Join(",", _scoreCheck.IneligibleCodes),
                result);

            return result;
        }
    }
}