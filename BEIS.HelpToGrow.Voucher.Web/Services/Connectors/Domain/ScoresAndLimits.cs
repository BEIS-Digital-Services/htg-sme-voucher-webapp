
namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class ScoresAndLimits
    {
        [JsonPropertyName("scoreGrade")]
        public string ScoreGrade { get; set; }

        [JsonPropertyName("protectScore")] 
        public int ProtectScore { get; set; }
    }
}