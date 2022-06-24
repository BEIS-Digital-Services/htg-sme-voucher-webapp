
namespace Beis.HelpToGrow.Voucher.Web.Models
{
    public static class Urls
    {
        public static Uri GetComparisonToolUrl(string learningPlatformUrl) => new(new Uri(learningPlatformUrl), "comparison-tool");

        public static Uri GetComparisonToolNoJsUrl(string learningPlatformUrl) => new(new Uri(learningPlatformUrl), "comparison-toolNoJs");

        public static Uri GetSatisfactionSurveyUrl(string learningPlatformUrl) => new(new Uri(learningPlatformUrl), "satisfaction-surve");

        public static Uri GetBusinessEligibilityUrl(string learningPlatformUrl) => new(new Uri(learningPlatformUrl), "eligibility");
    }
}