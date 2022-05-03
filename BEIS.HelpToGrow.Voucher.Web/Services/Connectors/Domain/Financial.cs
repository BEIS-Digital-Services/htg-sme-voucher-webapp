using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class Financial
    {
        [JsonPropertyName("financialData")]
        public FinancialData FinancialData { get; set; }
    }
}