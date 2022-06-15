


using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class CompanyHouseConnection: ICompanyHouseHttpConnection<CompanyHouseResponse>
    {
        private readonly IRestClientFactory _clientFactory;
        private readonly string _companyHouseApiUrl;
        private readonly string _apiToken;
        private readonly int _connectionTimeOut;
        
        public CompanyHouseConnection(
            IRestClientFactory clientFactory,
            string companyHouseApiUrl,
            string apiKey,
            string connectionTimeOut)
        {
            _clientFactory = clientFactory;
            _companyHouseApiUrl = companyHouseApiUrl;
            _apiToken = apiKey;
            _connectionTimeOut = int.Parse(connectionTimeOut);
        }

        public CompanyHouseResponse ProcessRequest(string companyId, HttpContext httpContext)
        {
            CompanyHouseResponse companyHouseResponse = null;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiToken + ":"));
            var client = _clientFactory.Create($"{_companyHouseApiUrl}{companyId}", _connectionTimeOut);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + credentials);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                companyHouseResponse = JsonSerializer.Deserialize<CompanyHouseResponse>(response.Content);
            }

            if (companyHouseResponse == null)
            {
                return new CompanyHouseResponse {HttpStatusCode = HttpStatusCode.NotFound};
            }

            companyHouseResponse.HttpStatusCode = response.StatusCode;
            
            return companyHouseResponse;
        }
    }
}