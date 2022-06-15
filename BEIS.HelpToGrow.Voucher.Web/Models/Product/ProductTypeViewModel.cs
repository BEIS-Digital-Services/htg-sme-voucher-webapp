
namespace BEIS.HelpToGrow.Voucher.Web.Models.Product
{
    public class ProductTypeViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select the type of business software you want to buy")]
        [NotDefault(ErrorMessage = "Select the type of business software you want to buy")]
        public long ProductType { get; set; }

        public settings_product_type SelectedProductType { get; set; }

        public List<settings_product_type> ProductTypeList { get; set; }
    }
}
