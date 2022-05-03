using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IEligibilityCheckResultRepository
    {
        Task AddCheckResult(eligibility_check_result checkResult);
    }
}