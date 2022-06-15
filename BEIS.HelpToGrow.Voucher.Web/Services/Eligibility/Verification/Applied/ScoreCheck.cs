
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class ScoreCheck : IVerifyScoreCheck
    {
        public IEnumerable<string> IneligibleCodes => new[] { "G", "I", "O", "N", "NA", "NR", "NT" };

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            IneligibleCodes.Contains(indesserCompanyResponse.ScoresAndLimits.ScoreGrade)
                ? Result.Fail(new EligibilityError(EligibilityErrorCode.ScoreCheck, $"Ineligible score found: '{indesserCompanyResponse.ScoresAndLimits.ScoreGrade}'"))
                : Result.Ok();
    }
}