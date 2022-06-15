
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyTotalAgreements : IVerify
    {
        int MinAgreementsCount { get; }
    }
}