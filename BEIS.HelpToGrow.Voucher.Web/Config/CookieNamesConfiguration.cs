namespace Beis.HelpToGrow.Voucher.Web.Config
{
    /// <summary>
    /// A class that implements a configuration for cookie names.
    /// </summary>
    public class CookieNamesConfiguration : ICookieNamesConfiguration
    {
        /// <summary>
        /// Gets or sets the dictionary of essential cookies.
        /// </summary>
        [JsonPropertyName("essential")]
        public Dictionary<string, string> Essential { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of non-essential cookies.
        /// </summary>
        [JsonPropertyName("nonEssential")]
        public Dictionary<string, string> NonEssential { get; set; }
    }
}