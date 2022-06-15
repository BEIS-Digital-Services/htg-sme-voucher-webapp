
namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public class DistributedCacheFactory : IDistributedCacheFactory
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheFactory(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public IDistributedCache Create()
        {
            return _distributedCache;
        }
    }
}