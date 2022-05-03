using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface IIndesserResponseService
    {
        Task<Result<long>> SaveAsync(IndesserCompanyResponse indesserCompanyResponse, long enterpriseId);
    }
}