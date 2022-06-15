
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class AccountFiling : AbstractVerification, IVerifyAccountFiling
    {
        public Characteristic Characteristic =>
            new(nameof(AccountFiling),
                Characteristics.AccountFiling,
                EligibilityErrorCode.AccountFiling);

        public string None => "0";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !value.Equals(None)
                ? Result.Fail(new EligibilityError(Characteristic, $"Account filing status found: {value}"))
                : Result.Ok();
    }
}