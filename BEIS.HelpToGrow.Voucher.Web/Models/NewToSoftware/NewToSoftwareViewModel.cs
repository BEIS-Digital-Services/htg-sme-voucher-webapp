
namespace Beis.HelpToGrow.Voucher.Web.Models.NewToSoftware
{
    public class NewToSoftwareViewModel
    {
        [Required]
        public string FirstTime { get; set; }

        public product SelectedProduct { get; set; }

        public string LearningPlatformUrl { get; set; }
    }
}