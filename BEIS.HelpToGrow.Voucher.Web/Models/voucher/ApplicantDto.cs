
namespace Beis.HelpToGrow.Voucher.Web.Models.Voucher
{
    public class ApplicantDto
    {
        public long EnterpriseId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasAcceptedTermsAndConditions { get; set; }
        public bool HasAcceptedPrivacyPolicy { get; set; }
        public bool HasAcceptedSubsidyControl { get; set; }
        public string EmailVerificationLink { get; set; }
        public bool IsVerified { get; set; }
        public bool HasProvidedMarketingConsent { get; set; }
        public bool HasProvidedMarketingConsentByPhone { get; set; }
    }
}