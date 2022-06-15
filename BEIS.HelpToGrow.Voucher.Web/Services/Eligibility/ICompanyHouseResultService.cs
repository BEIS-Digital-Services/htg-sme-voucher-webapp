
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface ICompanyHouseResultService
    {
        Task<Result> SaveAsync(CompanyHouseResponse response);
    }
}