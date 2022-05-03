using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IProductPriceDescriptionRepository
    {
        Task<product_price_base_metric_price> GetProductPriceBaseDescription(long productId);
    }
}