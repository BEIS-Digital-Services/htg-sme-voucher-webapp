namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyMortgagePresent : IVerify
    {
        string AMLR0006Value { get; }
    }
}