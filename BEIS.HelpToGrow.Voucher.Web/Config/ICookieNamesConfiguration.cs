using System.Collections.Generic;

namespace BEIS.HelpToGrow.Voucher.Web.Config
{
    /// <summary>
    /// An interface that defines the behaviour of a configuration for cookie names.
    /// </summary>
    public interface ICookieNamesConfiguration
    {
        /// <summary>
        /// Gets the dictionary of essential cookies.
        /// </summary>
        Dictionary<string, string> Essential { get; }

        /// <summary>
        /// Gets the dictionary of non-essential cookies.
        /// </summary>
        Dictionary<string, string> NonEssential { get; }
    }
}