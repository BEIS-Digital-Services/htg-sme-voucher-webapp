
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public interface INotifyService
    {
        Task<Result> SendVerifyEmailNotification(ApplicantDto applicant, string templateId =  null);
        Task<Result> SendVoucherToApplicant(UserVoucherDto userVoucher);
    }
}