using Newtonsoft.Json;

namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class SessionService : ISessionService
    {
        public void Set(string key, object value, HttpContext currentContext)
        {
            var serialized = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            currentContext.Session.Set(key, bytes);
        }

        public T Get<T>(string key, HttpContext currentContext)
        {
            if (!currentContext.Session.TryGetValue(key, out var bytes))
            {
                return default;
            }

            var value = Encoding.UTF8.GetString(bytes);

            return !string.IsNullOrWhiteSpace(value)
                ? JsonConvert.DeserializeObject<T>(value)
                : default;
        }

        public bool HasValidSession(HttpContext currentContext)
        {
            var isAvailable = currentContext?.Session.IsAvailable ?? false;

            return isAvailable && currentContext.Session.Keys.Any();
        }
    
        public void Remove(string key, HttpContext currentContext)
        {
            currentContext.Session.Remove(key);
        }
    }
}