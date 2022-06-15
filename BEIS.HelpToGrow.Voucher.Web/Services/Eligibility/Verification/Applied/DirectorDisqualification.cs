namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class DirectorDisqualification : AbstractVerification, IVerifyDirectorNonDisqualification
    {
        public Characteristic Characteristic => 
            new(nameof(DirectorDisqualification),
            Characteristics.DirectorDisqualification,
            EligibilityErrorCode.DirectorDisqualification);

        public string None => "0";

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string value) =>
            !value.Equals(None)
                ? Result.Fail(new EligibilityError(Characteristic, $"Director disqualification found: {value}"))
                : Result.Ok();
    }
}