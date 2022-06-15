
namespace BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain
{
    public interface IIndesserHttpConnection<T>
    {
        Result<T> ProcessRequest(string companyId, HttpContext httpContext);
    }
}