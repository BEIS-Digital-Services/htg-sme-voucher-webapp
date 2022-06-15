
using Newtonsoft.Json;
using RestSharp;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class IndesserErrorResponse
    {
        public HttpStatusCode HttpStatusCode { get; }
        public string Content { get; }
        public string ErrorMessage { get; }
        public string AsJson() => JsonConvert.SerializeObject(this);
        public IndesserErrorResponse(IRestResponse response)
        {
            HttpStatusCode = response.StatusCode;
            ErrorMessage = response.ErrorMessage;
            Content = response.Content;
        }
    }
}