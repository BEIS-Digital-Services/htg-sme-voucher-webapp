using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IVendorCompanyRepository
    {
        Task<vendor_company> GetVendorCompanySingle(long id);
        Task<List<vendor_company>> GetVendorCompanies();
    }
}