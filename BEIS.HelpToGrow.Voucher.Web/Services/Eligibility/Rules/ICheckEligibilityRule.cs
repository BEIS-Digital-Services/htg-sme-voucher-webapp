using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public interface ICheckEligibilityRule
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto);
    }
}