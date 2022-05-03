using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface INotifyService
    {
        Task<Result> SendVerifyEmailNotification(ApplicantDto applicant, string templateId =  null);
        Task<Result> SendVoucherToApplicant(UserVoucherDto userVoucher);
    }
}