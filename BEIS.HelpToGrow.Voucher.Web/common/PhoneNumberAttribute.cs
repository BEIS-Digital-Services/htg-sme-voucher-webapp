namespace Beis.HelpToGrow.Voucher.Web.common
{
    public class PhoneNumberAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// An actual UK phone number will start with 0 or +44 (the latter being the UK country code), or possibly just 44, followed by nine or ten digits.
        /// Matches +447222555555 | 07222555555 
        /// </summary>
        //@"^(?:0|\+?44)(?:\d\s?){9,14}$"
        public PhoneNumberAttribute() : base(@"^(?:0|\+?44)(?:\d\s?){9,14}$")
        {
        }
    }
}
