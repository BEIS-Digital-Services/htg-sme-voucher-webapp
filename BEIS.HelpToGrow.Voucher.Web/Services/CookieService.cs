
namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public class CookieService : ICookieService
    {
        private readonly ICookieNamesConfiguration _cookieNamesConfiguration;

        private readonly CookieOptions _cookieOptions = new()
        {
            Domain = Environment.GetEnvironmentVariable("COOKIE_DOMAIN"),
            Secure = true,
            SameSite = SameSiteMode.Strict,
            //HttpOnly = true,
            IsEssential = true,
            Expires = DateTime.Now.AddYears(2)
        };

        public CookieService(IOptions<CookieNamesConfiguration> cookieNamesConfiguration)
        {
            _cookieNamesConfiguration = cookieNamesConfiguration.Value;
        }

        public CookieBannerViewModel SyncCookieSelection(HttpRequest httpRequest, CookieBannerViewModel cookieBannerViewModel)
        {
            cookieBannerViewModel ??= new CookieBannerViewModel();

            cookieBannerViewModel.GoogleAnalyticsCookieAccepted =
                httpRequest.Cookies.ContainsKey(_cookieNamesConfiguration.NonEssential["HtGAnalyticsCookie"]) 
                ? httpRequest.Cookies[_cookieNamesConfiguration.NonEssential["HtGAnalyticsCookie"]]!.ToLower() 
                : string.Empty;

            cookieBannerViewModel.MarketingCookieAccepted =
                httpRequest.Cookies.ContainsKey(_cookieNamesConfiguration.NonEssential["HtGMarketingCookie"]) 
                ? httpRequest.Cookies[_cookieNamesConfiguration.NonEssential["HtGMarketingCookie"]]!.ToLower()
                : string.Empty;

            cookieBannerViewModel.IsCookieProcessed =
                httpRequest.Cookies.ContainsKey(_cookieNamesConfiguration.NonEssential["HtGAnalyticsCookie"]) &&
                httpRequest.Cookies.ContainsKey(_cookieNamesConfiguration.NonEssential["HtGMarketingCookie"]);

            cookieBannerViewModel.IsAllCookieAccepted =
                cookieBannerViewModel.IsGoogleAnalyticsCookieAccepted &&
                cookieBannerViewModel.IsMarketingCookieAccepted;

            cookieBannerViewModel.IsBannerClosed =
                httpRequest.Cookies.ContainsKey(_cookieNamesConfiguration.Essential["HtGcookieAcceptedCookie"]);

            return cookieBannerViewModel;
        }

        public async Task<bool> ProcessCookie(string cookieType, bool? isAccept, HttpResponse objResponse)
        {
            if (!isAccept.HasValue)
            {
                return await Task.FromResult(true);
            }

            if (cookieType != "act")
            {
                return await Task.FromResult(true);
            }

            objResponse.Cookies.Append(
                _cookieNamesConfiguration.NonEssential["HtGAnalyticsCookie"],
                isAccept.Value.ToString().ToLower(),
                _cookieOptions);
            
            objResponse.Cookies.Append(
                _cookieNamesConfiguration.NonEssential["HtGMarketingCookie"],
                isAccept.Value.ToString().ToLower(),
                _cookieOptions);
            
            objResponse.Cookies.Append(
                _cookieNamesConfiguration.Essential["HtGcookieAcceptedCookie"],
                "true",
                _cookieOptions);

            return await Task.FromResult(true); 
        }

        public async Task<bool> SaveCookiesPreferences(HttpContext objHttpContext)
        {
            var googleAnalyticsCookies = objHttpContext.Request.Form["GoogleAnalyticsCookieAccepted"];
            var marketingCookies = objHttpContext.Request.Form["MarketingCookieAccepted"];
            
            if (!string.IsNullOrWhiteSpace(googleAnalyticsCookies))
            {
                objHttpContext.Response.Cookies.Append(
                    _cookieNamesConfiguration.NonEssential["HtGAnalyticsCookie"],
                    googleAnalyticsCookies.ToString(), _cookieOptions);
            }

            if (!string.IsNullOrWhiteSpace(marketingCookies))
            {
                objHttpContext.Response.Cookies.Append(
                    _cookieNamesConfiguration.NonEssential["HtGMarketingCookie"], 
                    marketingCookies.ToString(), _cookieOptions);
            }

            objHttpContext.Response.Cookies.Append(
                _cookieNamesConfiguration.Essential["HtGcookieAcceptedCookie"],
                "true",
                _cookieOptions);

            return await Task.FromResult(true);
        }
    }
}