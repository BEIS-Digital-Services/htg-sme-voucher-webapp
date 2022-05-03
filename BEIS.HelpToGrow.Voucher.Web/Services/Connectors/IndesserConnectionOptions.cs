namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class IndesserConnectionOptions
    {
        
        public string INDESSER_TOKEN_URL { get; set; }
        public string TokenConnectionUrl => INDESSER_TOKEN_URL;

        public string INDESSER_CLIENT_ID { get; set; }
        public string ClientKey => INDESSER_CLIENT_ID;

        public string INDESSER_CLIENT_SECRET { get; set; }

        public string ClientSecret => INDESSER_CLIENT_SECRET;
        public string INDESSER_COMPANY_CHECK_URL { get; set; }
        public string CompanyCheckUrl => INDESSER_COMPANY_CHECK_URL;

        public string ConnectionTimeOut { get; set; }

    }
}