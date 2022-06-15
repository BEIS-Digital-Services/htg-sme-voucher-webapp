
namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class Identification
    {
        [JsonPropertyName("registeredOffice")]
        public RegisteredOffice RegisteredOffice { get; set; }
        [JsonPropertyName("companyNumber")]
        public string companyNumber { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}