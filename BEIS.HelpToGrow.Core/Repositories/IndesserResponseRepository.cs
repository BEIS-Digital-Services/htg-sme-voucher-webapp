using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class IndesserResponseRepository : IIndesserResponseRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public IndesserResponseRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public async Task<indesser_api_call_status> AddIndesserResponse(indesser_api_call_status indesserApiCallStatus)
        {
            await _context.indesser_api_call_statuses.AddAsync(indesserApiCallStatus);
            await _context.SaveChangesAsync();

            return indesserApiCallStatus;
        }
    }
}