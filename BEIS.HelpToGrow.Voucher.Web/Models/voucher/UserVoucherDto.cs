using System.Collections.Generic;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.Htg.VendorSme.Database.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Models.Voucher
{
    public class UserVoucherDto: VoucherErrorDetails
    {
        public List<product> ProductList { get; set; }
        public List<settings_product_type> ProductTypeList { get; set; }
        public product SelectedProduct { get; set; }
        public settings_product_type SelectedProductType { get; set; }
        public string VendorName { get; set; }
        public string FirstTime { get; set; }
        public string ExistingCustomer { get; set; }
        public string MajorUpgrade { get; set; }
        public string CompanySize { get; set; }
        public int EmployeeNumbers { get; set; }
        public string CompanyHouseNumber { get; set; }
        public string HasCompanyHouseNumber { get; set; }
        public string FCANumber { get; set; }
        public string HasFCANumber { get; set; }
        public string Addons { get; set; }
        public string WorkEmail { get; set; }
        public string ConsentTermsConditions { get; set; }
        public ApplicantDto ApplicantDto { get; set; } = new();
        public CompanyHouseResponse CompanyHouseResponse { get; set; }
        public string voucherCode { get; set; }
        public string tokenPurchaseLink { get; set; }
        public CookieBannerViewModel CookieBannerViewModel { get; set; }
    }
}