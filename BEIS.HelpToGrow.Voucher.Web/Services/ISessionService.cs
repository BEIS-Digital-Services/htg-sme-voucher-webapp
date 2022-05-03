using Microsoft.AspNetCore.Http;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface ISessionService
    {
        void Set(string key, object value, HttpContext currentContext);
        T Get<T>(string key, HttpContext currentContext);
        bool HasValidSession(HttpContext currentContext);
        void Remove(string key, HttpContext currentContext);
    }
}