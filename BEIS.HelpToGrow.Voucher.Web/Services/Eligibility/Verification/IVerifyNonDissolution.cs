namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyNonDissolution : IVerify
    {
        string ActiveTradingStatus { get; }
    }
}