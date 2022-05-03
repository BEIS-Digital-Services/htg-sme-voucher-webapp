using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IProductPriceRepository
    {
        Task<product_price> GetByProductId(long productId);
    }
}