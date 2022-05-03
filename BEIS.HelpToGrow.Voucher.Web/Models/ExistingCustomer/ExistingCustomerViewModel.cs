using System.ComponentModel.DataAnnotations;
using Beis.Htg.VendorSme.Database.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public class ExistingCustomerViewModel
    {
        [Required]
        public string ExistingCustomer { get; set; }

        public string VendorName { get; set; }
    }
}