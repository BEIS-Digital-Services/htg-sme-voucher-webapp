
using RestSharp;

namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public interface IRestClientFactory
    {
        IRestClient Create(string url, int connectionTimeOut);
    }
}