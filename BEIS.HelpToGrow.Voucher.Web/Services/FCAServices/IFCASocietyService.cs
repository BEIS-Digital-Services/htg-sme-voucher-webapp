
namespace Beis.HelpToGrow.Voucher.Web.Services.FCAServices
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