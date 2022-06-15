namespace Beis.HelpToGrow.Voucher.Web.Models
{
    public class CookieBannerViewModel
    {
        public string GoogleAnalyticsCookieAccepted { get; set; }
        public string MarketingCookieAccepted { get; set; }
        public bool IsGoogleAnalyticsCookieAccepted => GoogleAnalyticsCookieAccepted == "true";
        public bool IsMarketingCookieAccepted => MarketingCookieAccepted == "true";
        public bool IsAllCookieAccepted { get; set; }
        public bool IsBannerClosed { get; set; }
        public bool IsCookieProcessed { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string PageName { get; set; }

        public long ProductId { get; set; }

        public long ProductType { get; set; }
    }
}