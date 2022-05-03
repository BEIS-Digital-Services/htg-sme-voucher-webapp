using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class EligibilityCheckResultRepository : IEligibilityCheckResultRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public EligibilityCheckResultRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task AddCheckResult(eligibility_check_result checkResult)
        {
            await _context.eligibility_check_results.AddAsync(checkResult);
            await _context.SaveChangesAsync();
        }
    }
}