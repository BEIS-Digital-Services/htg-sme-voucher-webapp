

namespace BEIS.HelpToGrow.Voucher.Web.Common
{
    /// <summary>
    /// Requires the format [first level domain]@p[second level domain].[third level domain (length 2 to 20)]
    /// </summary>
    public class EmailAttribute : RegularExpressionAttribute
    {
        //@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"
        public EmailAttribute() : base(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,20})+)$")
        {
        }
    }
}