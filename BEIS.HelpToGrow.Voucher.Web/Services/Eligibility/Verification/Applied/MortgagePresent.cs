using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class MortgagePresent : IVerifyMortgagePresent
    {
        private static Characteristic Characteristic => new(nameof(MortgagePresent), Characteristics.MortgagePresent, EligibilityErrorCode.MortgagePresent);

        public string AMLR0006Value => "1";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            AMLR0006Value.Equals(indesserCompanyResponse.Characteristic(Characteristic.Code).Value.Value)
                ? Result.Ok()
                : Result.Fail(new EligibilityError(Characteristic, $"Mortgage check value found is: '{indesserCompanyResponse.Characteristic(Characteristic.Code).Value.Value}'"));
    }
}