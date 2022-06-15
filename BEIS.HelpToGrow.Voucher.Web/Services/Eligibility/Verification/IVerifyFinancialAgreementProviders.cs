
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyFinancialAgreementProviders : IVerify
    {
        int MinFinancialAgreementProviders { get; }
    }
}