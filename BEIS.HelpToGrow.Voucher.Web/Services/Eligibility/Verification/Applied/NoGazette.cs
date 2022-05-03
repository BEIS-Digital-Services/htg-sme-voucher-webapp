using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class NoGazette : AbstractVerification, IVerifyNoGazette
    {
        public Characteristic Characteristic =>
            new(nameof(NoGazette),
                Characteristics.GazetteData,
                EligibilityErrorCode.Gazette);

        public string None => "0";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !value.Equals(None)
                ? Result.Fail(new EligibilityError(Characteristic, $"Gazette data found: {value}"))
                : Result.Ok();
    }
}