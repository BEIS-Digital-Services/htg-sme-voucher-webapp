
using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class Characteristic
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")] 
        public string Value { get; set; }
    }
}