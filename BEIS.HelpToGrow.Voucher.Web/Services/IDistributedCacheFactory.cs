using Microsoft.Extensions.Caching.Distributed;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface IDistributedCacheFactory
    {
        IDistributedCache Create();
    }
}