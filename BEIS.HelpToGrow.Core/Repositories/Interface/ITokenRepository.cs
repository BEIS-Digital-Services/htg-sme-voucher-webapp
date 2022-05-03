using System.Collections.Generic;
using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface ITokenRepository
    {
        Task AddToken(token token);
        Task<token> GetToken(long enterpriseId, long productId);
        Task<List<token>> GetEnterpriseNotRedeemedToken(int unactionedDays, bool reminder1, bool reminder2, bool reminder3);
        Task UpdateReminderStatus(long tokenId, bool reminder1, bool reminder2, bool reminder3);
        Task<token> GetToken(long enterpriseId);
        Task<token> UpdateToken(token token);
    }
}