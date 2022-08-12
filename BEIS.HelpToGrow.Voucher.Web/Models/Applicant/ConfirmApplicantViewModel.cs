
namespace Beis.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class ConfirmApplicantViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string SoftwareProduct { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyNumber { get; set; }
        
        public bool HasAcceptedTermsAndConditions { get; set; }
        public bool HasAcceptedPrivacyPolicy { get; set; }
        public bool HasAcceptedSubsidyControl { get; set; }
        public bool HasProvidedMarketingConsent { get; set; }
        public string MarketingConsentResponse => HasProvidedMarketingConsent ? "Yes" : "No";
        public Uri ComparisonToolURL { get; set; }
        public string ProductPrice { get; set; }
    }
}