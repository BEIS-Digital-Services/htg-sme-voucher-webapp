
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public interface ICheckEligibilityRule
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto);
    }
}