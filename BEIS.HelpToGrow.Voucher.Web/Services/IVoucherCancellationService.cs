using BEIS.HelpToGrow.Core.Enums;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface IVoucherCancellationService
    {
        Task<CancellationResponse> CancelVoucherFromEmailLink(long enterpriseId, string emailAddress);
    }
}