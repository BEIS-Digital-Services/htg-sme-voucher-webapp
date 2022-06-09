using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using System.Text.Json.Serialization;

namespace BEIS.HelpToGrow.Voucher.Web.Config
{
    public class CompanyHouseHealthCheckConfiguration
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyStatus { get; set; }
    }
}
