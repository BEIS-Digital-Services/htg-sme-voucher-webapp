using System.Linq;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class AbnormalFiling : AbstractVerification, IVerifyNoAbnormalFiling
    {
        private static Characteristic Characteristic =>
            new(nameof(AbnormalFiling),
                Characteristics.AbnormalFiling,
                EligibilityErrorCode.AbnormalFiling);

        public string None => "0";
        public string NotApplicable => "_";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !new[] {None, NotApplicable}.Contains(value)
                ? Result.Fail(new EligibilityError(Characteristic, $"Abnormal filing status found: {value}"))
                : Result.Ok();
    }
}