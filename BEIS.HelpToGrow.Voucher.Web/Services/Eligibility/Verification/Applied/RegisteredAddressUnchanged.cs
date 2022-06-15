
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class RegisteredAddressUnchanged : AbstractVerification, IVerifyRegisteredAddressUnchanged
    {
        public Characteristic Characteristic =>
            new(nameof(RegisteredAddressUnchanged),
            Characteristics.CompanyAddressChanged,
            EligibilityErrorCode.CompanyAddressChanged);

        public IEnumerable<string> Unchanged => new List<string>{ "M", "C", "_" };

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !Unchanged.Contains(value)
                ? Result.Fail(new EligibilityError(Characteristic, $"Company registered office address has changed status found: {value}"))
                : Result.Ok();
    }
}