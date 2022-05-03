using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Interfaces
{
    public interface IVoucherGenerationService
    {
        public Task<string> GenerateVoucher(vendor_company vendorCompany, enterprise enterprise, product product);
        public string GenerateSetCode(int length);
    }
}