
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public interface IEligibilityCheckResultService
    {
        Task<Result> SaveAsync(Check check, Result<long> indesserCallSavedResult);
    }
}