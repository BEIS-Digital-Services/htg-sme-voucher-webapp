using Beis.Htg.VendorSme.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<product> GetProductSingle(long id);
        Task<List<product>> GetProducts();
        Task<List<settings_product_type>> ProductTypes();
    }
}