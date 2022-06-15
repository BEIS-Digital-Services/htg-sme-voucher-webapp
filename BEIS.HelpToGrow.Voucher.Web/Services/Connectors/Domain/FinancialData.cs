using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class FinancialData
    {
        [JsonPropertyName("numberofEmployees")]
        public double NumberofEmployees { get; set; }
    }
}