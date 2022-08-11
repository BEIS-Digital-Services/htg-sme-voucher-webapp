
namespace Beis.HelpToGrow.Voucher.Web.Models
{
    public class TermsConditionsViewModel : UsefulLinksViewModel
    {
        public string SelectedProduct { get; set; }

   
        [Required]
        public bool TermsAndConditionsAccepted { get; set; }
        [Required]
        public bool PrivacyPolicyAccepted { get; set; }
        [Required]
        public bool SubsidyControlAccepted { get; set; }

        public bool IsIncomplete => !(TermsAndConditionsAccepted && PrivacyPolicyAccepted && SubsidyControlAccepted);



    }

}