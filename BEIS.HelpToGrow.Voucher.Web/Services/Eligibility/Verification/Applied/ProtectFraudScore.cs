using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class ProtectFraudScore : IVerifyProtectFraudScore
    {
        public int MinScoreAllowed => -199;

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            indesserCompanyResponse.ScoresAndLimits.ProtectScore < MinScoreAllowed
                ? Result.Fail(new EligibilityError(EligibilityErrorCode.ProtectFraudScore, $"Low protect score found: '{indesserCompanyResponse.ScoresAndLimits.ProtectScore}'"))
                : Result.Ok();
    }
}