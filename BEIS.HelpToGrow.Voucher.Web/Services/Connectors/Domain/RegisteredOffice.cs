using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class RegisteredOffice
    {
        [JsonPropertyName("line1")]
        public string line1 { get; set; }
        [JsonPropertyName("line2")]
        public string line2 { get; set; }
        [JsonPropertyName("line3")]
        public string line3 { get; set; }
        [JsonPropertyName("line4")]
        public string line4 { get; set; }
        [JsonPropertyName("postcode")]
        public string postcode { get; set; }

    }
}