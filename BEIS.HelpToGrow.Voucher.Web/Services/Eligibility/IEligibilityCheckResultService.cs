using System.Threading.Tasks;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface IEligibilityCheckResultService
    {
        Task<Result> SaveAsync(Check check, Result<long> indesserCallSavedResult);
    }
}