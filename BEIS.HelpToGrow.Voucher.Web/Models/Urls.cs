using System;

namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public static class Urls
    {
        public static string LearningPlatformUrl =>
            Environment.GetEnvironmentVariable("LEARNING_PLATFORM_URL").ToLower()
            ?? throw new NullReferenceException("Missing Learning Platform URL");

        public static Uri ComparisonToolUrl => new(new Uri(LearningPlatformUrl), "comparison-tool");

        public static Uri ComparisonToolNoJsUrl => new(new Uri(LearningPlatformUrl), "comparison-toolNoJs");

        public static Uri SatisfactionSurveyUrl => new(new Uri(LearningPlatformUrl), "satisfaction-survey");

        public static Uri BusinessEligibilityUrl => new(new Uri(LearningPlatformUrl), "eligibility");

        public static string EmailVerificationUrl =>
            Environment.GetEnvironmentVariable("EMAIL_VERIFICATION_LINK_URL").ToLower()
                 ?? throw new NullReferenceException("Missing Learning Platform URL");
    }
}