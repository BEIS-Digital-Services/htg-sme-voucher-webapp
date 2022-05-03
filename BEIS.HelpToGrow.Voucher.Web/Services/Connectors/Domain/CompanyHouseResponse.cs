using System.Net;
using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public class CompanyHouseResponse
    {
        [JsonPropertyName("company_number")]
        public string CompanyNumber { get; set; }
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("company_status")]
        public string CompanyStatus { get; set; }
        [JsonPropertyName("sic_codes")]
        public string[] SicCodes { get; set; }
        [JsonPropertyName("date_of_creation")]
        public string CreationDate { get; set; }
        [JsonPropertyName("registered_office_address")]
        public RegisteredOfficeAddress RegisteredOfficeAddress { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        [JsonPropertyName("type")]
        public string CompanyType { get; set; }
        [JsonPropertyName("has_insolvency_history")]
        public bool HasInsolvencyHistory { get; set; }
        [JsonPropertyName("jurisdiction")]
        public string Jurisdiction { get; set; }
        [JsonPropertyName("registered_office_is_in_dispute")]
        public bool RegisteredOfficeDisputed { get; set; }
        [JsonPropertyName("undeliverable_registered_office_address")]
        public bool UndeliverableRegisteredOfficeAddress { get; set; }
    }
}