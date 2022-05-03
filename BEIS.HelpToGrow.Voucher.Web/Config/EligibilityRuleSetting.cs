using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Config
{
    public class EligibilityRuleSetting
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("contributesToFailCount")]
        public bool ContributesToFailCount { get; set; }
        [JsonPropertyName("contributesToReviewCount")]
        public bool ContributesToReviewCount { get; set; }
    }
}