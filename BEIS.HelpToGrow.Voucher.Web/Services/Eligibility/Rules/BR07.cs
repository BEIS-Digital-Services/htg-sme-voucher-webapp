
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR07 : CheckEligibilityRule
    {
        private readonly ILogger<BR07> _logger;
        private readonly IVerifyDirectorNonDisqualification _directorNonDisqualification;
        public override string Name => "Disqualification";
        public override string Description => "None of the current officers in the company are in the disqualified director database.";

        public BR07(ILogger<BR07> logger, IVerifyDirectorNonDisqualification directorNonDisqualification)
        {
            _logger = logger;
            _directorNonDisqualification = directorNonDisqualification;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _directorNonDisqualification.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, director disqualification status:{@Status}, result:{@Result}",
                nameof(BR07),
                indesserCompanyResponse.Identification?.companyNumber,
                _directorNonDisqualification.None,
                result);

            return result;
        }
    }
}