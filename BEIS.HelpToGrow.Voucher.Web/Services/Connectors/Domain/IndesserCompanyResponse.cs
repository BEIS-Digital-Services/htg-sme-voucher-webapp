
namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class IndesserCompanyResponse
    {
        [JsonPropertyName("identification")]
        public Identification Identification { get; set; }
        [JsonPropertyName("characteristics")]
        public List<Characteristic> Characteristics { get; set; }
        [JsonPropertyName("scoresAndLimits")]
        public ScoresAndLimits ScoresAndLimits { get; set; }
        [JsonPropertyName("financials")]
        public IEnumerable<Financial> Financials { get; set; }
    }
}
