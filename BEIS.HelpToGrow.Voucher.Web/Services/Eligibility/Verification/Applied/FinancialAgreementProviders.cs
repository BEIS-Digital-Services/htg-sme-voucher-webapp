
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class FinancialAgreementProviders : AbstractVerification, IVerifyFinancialAgreementProviders
    {
        public Characteristic Characteristic =>
            new(nameof(FinancialAgreementProviders), Characteristics.FinancialAgreementProviders, EligibilityErrorCode.FinancialAgreementProviders);
        
        public int MinFinancialAgreementProviders => 1;

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value)
        {
            if (!int.TryParse(value, out var financialAgreementProviders))
            {
                return Result.Fail(new EligibilityError(Characteristic, $"Financial agreement providers: '{value}'"));
            }
            
            return financialAgreementProviders < MinFinancialAgreementProviders
                ? Result.Fail(new EligibilityError(Characteristic, $"Financial agreement providers: '{value}'"))
                : Result.Ok();
        }
    }
}