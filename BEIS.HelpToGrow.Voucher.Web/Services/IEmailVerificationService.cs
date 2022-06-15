
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public interface IEmailVerificationService
    {
        string GetVerificationCode(UserVoucherDto userVoucher);
        Task<Result> SendVerifyEmailNotificationAsync(ApplicantDto applicant, string templateId = null);
        Task<Result> VerifyEnterpriseFromCodeAsync(string verificationCode);
        Task<Result> CreateOrUpdateEnterpriseDetailsAsync(UserVoucherDto userVoucher);
        Task<bool> CompanyNumberIsUnique(string companyNumber, string fcaNumber);

        Task<UserVoucherDto> GetUserVoucherFromEnterpriseAsync(long enterpriseId, long productId);
    }
}