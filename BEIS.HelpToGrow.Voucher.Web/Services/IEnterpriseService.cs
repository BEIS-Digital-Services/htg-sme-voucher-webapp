
namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface IEnterpriseService
    {
        Task<Result> CreateOrUpdateEnterpriseDetailsAsync(UserVoucherDto userVoucher);
        Task<Result> SetEnterpriseAsVerified(long enterpriseId);
        Task<bool> CompanyNumberIsUnique(string companyNumber);
        Task<bool> FcaNumberIsUnique(string fcaNumber);
        Task<UserVoucherDto> GetUserVoucherFromEnterpriseAsync(long enterpriseId, long productId);
        Task<Result> SetEligibilityStatusAsync(EligibilityStatus eligibility);
        Task<EligibilityStatus> GetEligibilityStatusAsync();
        Task Unsubscribe(long enterpriseId, string emailAddress);
    }
}