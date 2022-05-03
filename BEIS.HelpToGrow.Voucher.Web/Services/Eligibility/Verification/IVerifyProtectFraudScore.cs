namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyProtectFraudScore : IVerify
    {
        int MinScoreAllowed { get; }
    }
}