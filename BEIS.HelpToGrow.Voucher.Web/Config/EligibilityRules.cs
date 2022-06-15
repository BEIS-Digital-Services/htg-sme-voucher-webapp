
using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class EligibilityRules
    {
        [JsonPropertyName("core")]
        public EligibilityRulesSection Core { get; set; }
        
        [JsonPropertyName("additional")]
        public EligibilityRulesSection Additional { get; set; }
    }
}