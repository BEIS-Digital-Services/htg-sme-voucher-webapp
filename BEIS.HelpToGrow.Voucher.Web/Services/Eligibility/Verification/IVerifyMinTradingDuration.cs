
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyMinTradingDuration
    {
        Result Verify(IndesserCompanyResponse indesserCompanyResponse, int minTradingDurationInMonths);
    }
}