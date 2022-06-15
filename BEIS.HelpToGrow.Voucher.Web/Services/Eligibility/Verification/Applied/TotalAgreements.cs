
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class TotalAgreements : AbstractVerification, IVerifyTotalAgreements
    {
        public Characteristic Characteristic =>
            new(nameof(TotalAgreements),
                Characteristics.TotalAgreements,
                EligibilityErrorCode.TotalAgreements);

        public int MinAgreementsCount => 1;

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value)
        {
            var eligibilityError = new EligibilityError(Characteristic, $"Total agreements found: '{value}'");

            if (!int.TryParse(value, out var totalAgreements))
            {
                return Result.Fail(eligibilityError);
            }

            return totalAgreements < MinAgreementsCount
                ? Result.Fail(eligibilityError)
                : Result.Ok();
        }
    }
}