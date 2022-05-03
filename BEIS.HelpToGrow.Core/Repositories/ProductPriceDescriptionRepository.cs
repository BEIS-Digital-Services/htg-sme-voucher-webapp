using System.Linq;
using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class ProductPriceDescriptionRepository : IProductPriceDescriptionRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductPriceDescriptionRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<product_price_base_metric_price> GetProductPriceBaseDescription(long productId)
        {
            var productPrice = await _context.product_prices
                .Include(_ => _.product_price_base_metric_prices)
                .ThenInclude(_ => _.product_price_base_description)
                .FirstOrDefaultAsync(_ => _.productid == productId);

            return productPrice.product_price_base_metric_prices.First();
        }
    }
}