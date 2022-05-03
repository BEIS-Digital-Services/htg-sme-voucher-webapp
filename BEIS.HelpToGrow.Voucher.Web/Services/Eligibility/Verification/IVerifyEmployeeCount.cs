using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyEmployeeCount
    {
        int EmployeeCountMin { get; }
        int EmployeeCountMax { get; }

        Result Verify(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto);
    }
}