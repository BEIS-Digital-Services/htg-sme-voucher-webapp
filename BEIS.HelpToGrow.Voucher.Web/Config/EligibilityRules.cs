using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class EligibilityRules
    {
        [JsonPropertyName("core")]
        [Required]
        public EligibilityRulesSection Core { get; set; }
        
        [JsonPropertyName("additional")]
        [Required]
        public EligibilityRulesSection Additional { get; set; }
    }
}