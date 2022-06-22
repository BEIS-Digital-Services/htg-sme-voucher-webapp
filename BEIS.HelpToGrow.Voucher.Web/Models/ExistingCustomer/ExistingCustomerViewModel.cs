
namespace Beis.HelpToGrow.Voucher.Web.Models
{
    public class ExistingCustomerViewModel
    {
        [Required]
        public string ExistingCustomer { get; set; }

        public string VendorName { get; set; }
    }
}