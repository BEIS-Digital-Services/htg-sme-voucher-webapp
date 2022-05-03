using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface ICheckEligibility
    {
        Result<Check> Check(UserVoucherDto userVoucherDto, IndesserCompanyResponse indesserCompanyResponse);
    }
}