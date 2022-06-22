
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions
{
    public static class UserVoucherExtensions
    {
        public static bool ToBoolean(this string response) =>
            !string.IsNullOrWhiteSpace(response) &&
            response.Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
    }
}