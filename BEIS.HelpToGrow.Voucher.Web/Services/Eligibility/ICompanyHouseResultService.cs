using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface ICompanyHouseResultService
    {
        Task<Result> SaveAsync(CompanyHouseResponse response);
    }
}