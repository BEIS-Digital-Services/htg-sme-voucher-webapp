
namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public class MajorUpgradeViewModel
    {
        [Required]
        public string MajorUpgrade { get; set; }

        public product SelectedProduct { get; set; }
    }
}
