
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class ActivelyTrading : AbstractVerification, IVerifyNonDissolution
    {
        public Characteristic Characteristic =>
            new(nameof(ActivelyTrading),
                Characteristics.ActivelyTrading,
                EligibilityErrorCode.CompanyInactive);

        public string ActiveTradingStatus => "0";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string characteristicValue) =>
            !characteristicValue.Equals(ActiveTradingStatus)
                ? Result.Fail(new EligibilityError(Characteristic, $"'{ActiveTradingStatus}' trading status expected but found {characteristicValue}"))
                : Result.Ok();
    }
}