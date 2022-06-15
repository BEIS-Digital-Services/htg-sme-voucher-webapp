
using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class ConnectionToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("timeAcquired")]
        public DateTime TimeAcquired { get; set; }
        [JsonPropertyName("currentStatus")]
        public string CurrentStatus { get; set; }
    }
}