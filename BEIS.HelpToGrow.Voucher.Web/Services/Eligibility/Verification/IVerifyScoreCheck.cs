
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyScoreCheck : IVerify
    {
        IEnumerable<string> IneligibleCodes { get; }
    }
}