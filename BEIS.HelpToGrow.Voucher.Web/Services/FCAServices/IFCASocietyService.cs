using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;

namespace BEIS.HelpToGrow.Voucher.Web.Services.FCAServices
{
    public interface IFCASocietyService
    {
        Task<FCASociety> GetSociety(string societyNumber);
        Task LoadFCASocieties();
    }

    public interface IProductPriceService
    {
        Task<string> GetProductPrice(long id);
    }
}