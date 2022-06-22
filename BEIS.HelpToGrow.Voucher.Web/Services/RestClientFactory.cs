using RestSharp;

namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(string url, int connectionTimeOut) =>
            new RestClient(url)
            {
                Timeout = connectionTimeOut
            };
    }
}