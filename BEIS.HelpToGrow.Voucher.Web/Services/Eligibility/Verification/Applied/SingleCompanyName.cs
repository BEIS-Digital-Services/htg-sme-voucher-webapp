using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class SingleCompanyName : AbstractVerification, IVerifySingleCompanyName
    {
        public Characteristic Characteristic =>
            new(nameof(SingleCompanyName),
                Characteristics.MultipleNameMatches,
                EligibilityErrorCode.MultipleMatches);
        
        public string None => "0";
        public string NotApplicable => "_";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !value.Equals(None) && !value.Equals(NotApplicable)
                ? Result.Fail(new EligibilityError(Characteristic, $"Multiple matches found status: {value}"))
                : Result.Ok();
    }
}