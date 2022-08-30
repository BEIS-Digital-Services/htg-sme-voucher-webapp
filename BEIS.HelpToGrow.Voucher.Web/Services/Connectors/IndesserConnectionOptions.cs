
namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class IndesserConnectionOptions
    {
        [Required]
        [Url]
        public string IndesserTokenUrl { get; set; }
        public string TokenConnectionUrl => IndesserTokenUrl;

        [Required]
        public string IndesserClientId { get; set; }
        public string ClientKey => IndesserClientId;

        [Required]
        public string IndesserClientSecret { get; set; }

        public string ClientSecret => IndesserClientSecret;
        
        [Required]
        public string IndesserCompanyCheckUrl { get; set; }
        public string CompanyCheckUrl => IndesserCompanyCheckUrl;

        [Required]
        [Range(0, 100000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public string IndesserConnectionTimeOut { get; set; }

        public string ConnectionTimeOut => IndesserConnectionTimeOut;
    }
}