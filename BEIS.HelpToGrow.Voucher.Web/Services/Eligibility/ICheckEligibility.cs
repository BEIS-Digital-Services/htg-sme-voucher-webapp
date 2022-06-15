
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface ICheckEligibility
    {
        Result<Check> Check(UserVoucherDto userVoucherDto, IndesserCompanyResponse indesserCompanyResponse);
    }
}