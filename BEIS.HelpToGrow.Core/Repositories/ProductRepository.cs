
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public ProductRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public Task<List<settings_product_type>> ProductTypes()
        {
            return _context.settings_product_types.ToListAsync();
        }

        public async Task<product> GetProductSingle(long id)
        {
            return await _context.products.FirstOrDefaultAsync(t => t.product_id == id);
        }

        public async Task<List<product>> GetProducts()
        {
            return await _context.products.ToListAsync();
        }
    }
}