using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IEnterpriseRepository
    {
        Task AddEnterprise(enterprise enterprise);
        Task<enterprise> GetEnterprise(long enterpriseId);
        Task<enterprise> GetEnterpriseByCompanyNumber(string companyNumber);

        Task<enterprise> GetEnterpriseByFCANumber(string fcaNumber);
        Task<enterprise> UpdateEnterprise(enterprise enterprise);
    }
}