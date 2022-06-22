
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyEmployeeCount
    {
        int EmployeeCountMin { get; }
        int EmployeeCountMax { get; }

        Result Verify(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto);
    }
}