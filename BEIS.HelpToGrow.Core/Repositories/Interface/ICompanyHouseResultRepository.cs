using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface ICompanyHouseResultRepository
    {
        Task SaveCompanyHouseResult(companies_house_api_result result);
        Task<bool> Exists(string companyHouseNumber);
    }
}