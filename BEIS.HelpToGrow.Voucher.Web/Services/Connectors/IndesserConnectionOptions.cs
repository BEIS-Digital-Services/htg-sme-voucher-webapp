namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class IndesserConnectionOptions
    {
        
        public string IndesserTokenUrl { get; set; }
        public string TokenConnectionUrl => IndesserTokenUrl;

        public string IndesserClientId { get; set; }
        public string ClientKey => IndesserClientId;

        public string IndesserClientSecret { get; set; }

        public string ClientSecret => IndesserClientSecret;
        public string IndesserCompanyCheckUrl { get; set; }
        public string CompanyCheckUrl => IndesserCompanyCheckUrl;

        public string IndesserConnectionTimeOut { get; set; }

        public string ConnectionTimeOut => IndesserConnectionTimeOut;
    }
}