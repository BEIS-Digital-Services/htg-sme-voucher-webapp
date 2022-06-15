
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface IIndesserResponseService
    {
        Task<Result<long>> SaveAsync(IndesserCompanyResponse indesserCompanyResponse, long enterpriseId);
    }
}