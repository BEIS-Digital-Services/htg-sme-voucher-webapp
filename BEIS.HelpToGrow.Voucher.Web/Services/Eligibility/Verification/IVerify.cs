
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerify
    {
        Result Verify(IndesserCompanyResponse indesserCompanyResponse);
    }
}