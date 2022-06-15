
namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public interface ICompanyHouseHttpConnection<out T>
    {
        T ProcessRequest(string companyId, HttpContext httpContext);
    }
}