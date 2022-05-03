using System;

namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public class UsefulLinksViewModel
    {
        public Uri LearningPlatformURL => new(Urls.LearningPlatformUrl);
        public Uri ComparisonToolURL => Urls.ComparisonToolUrl;

        public Uri ComparisonToolNoJsURL => Urls.ComparisonToolNoJsUrl;

        public Uri LearningPlatformEligibilityURL => Urls.BusinessEligibilityUrl;
        public bool IsTermsConditionsHidden { get; set; }
        public bool IsPrivacyPolicyHidden { get; set; }
        public bool IsHelpToGrowHidden { get; set; }
        public bool IsApplyForDiscountHidden { get; set; }
        public bool IsGeneralGuidanceHidden { get; set; }
        public bool IsGetInTouchHidden { get; set; }
    }
}