
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class HoldingCompanyRegistration : AbstractVerification, IVerifyHoldingCompanyRegistration
    {
        public Characteristic Characteristic =>
            new(nameof(HoldingCompanyRegistration),
                Characteristics.HoldingCompanyRegistration,
                EligibilityErrorCode.NonUnitedKingdomHoldingCompanyRegistration);

        public string None => "0";
        public string NotApplicable => "_";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !value.Equals(None) && !value.Equals(NotApplicable)
                ? Result.Fail(new EligibilityError(Characteristic, $"Non UK holding company registration status found: {value}"))
                : Result.Ok();
    }
}