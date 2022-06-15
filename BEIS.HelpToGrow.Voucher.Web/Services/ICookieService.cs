
namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface ICookieService
    {
        public CookieBannerViewModel SyncCookieSelection(HttpRequest httpRequest, CookieBannerViewModel cookieBannerViewModel);
        public Task<bool> ProcessCookie(string cookieType, bool? isAccept, HttpResponse objResponse);

        public Task<bool> SaveCookiesPreferences(HttpContext objHttpContext);
    }
}
