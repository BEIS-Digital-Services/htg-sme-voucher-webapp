
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public abstract class CheckEligibilityRule : ICheckEligibilityRule
    {
        public string Id => GetType().Name;
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto);
    }
}