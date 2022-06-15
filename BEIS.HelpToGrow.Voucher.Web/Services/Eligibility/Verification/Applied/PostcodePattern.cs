


namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class PostcodePattern : AbstractVerification, IVerifyPostcodePattern
    {
        private static Characteristic Characteristic => new(nameof(PostcodePattern), Characteristics.Postcode, EligibilityErrorCode.NonUnitedKingdomPostcode);

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse) =>
            Verify(indesserCompanyResponse, Characteristic).IsFailed
                ? Verify(indesserCompanyResponse.Identification?.RegisteredOffice?.postcode)
                : Verify(indesserCompanyResponse, Characteristic);

        protected override Result Verify(string postcode) =>
            !PostcodePatternVerification.IsValidPostcode(postcode)
                ? Result.Fail(new EligibilityError(Characteristic, $"Not recognised as a valid UK postcode: {postcode}"))
                : Result.Ok();

        public static class PostcodePatternVerification
        {
            private const string PostCodeRegex = @"^([A-Z][A-HJ-Y]?\d[A-Z\d]? ?\d[A-Z]{2}|GIR ?0A{2})$";

            public static bool IsValidPostcode(string postcode)
            {
                return postcode is not null && Regex.Match(postcode, PostCodeRegex).Success;
            }
        }
    }
}