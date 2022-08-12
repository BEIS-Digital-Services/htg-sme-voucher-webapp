
namespace Beis.HelpToGrow.Voucher.Web.Models
{
    public class TermsConditionsViewModel : UsefulLinksViewModel
    {
        public string SelectedProduct { get; set; }

   
        [Required]
        public bool TermsAndConditions { get; set; }
        [Required]
        public bool PrivacyPolicy { get; set; }
        [Required]
        public bool SubsidyControl { get; set; }

        public bool IsIncomplete => !(TermsAndConditions && PrivacyPolicy && SubsidyControl);



    }

}