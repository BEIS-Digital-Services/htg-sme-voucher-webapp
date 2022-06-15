
namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public class FakeRedisCache : IDistributedCache
    {
        private Exception _exception;

        private readonly Dictionary<string, byte[]> _cache = new();

        public FakeRedisCache Throwing(Exception ex)
        {
            _exception = ex;
            return this;
        }

        private void CheckThrown()
        {
            if (_exception != null)
            {
                throw _exception;
            }
        }

        public byte[] Get(string key)
        {
            CheckThrown();
            return _cache[key];
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = new())
        {
            CheckThrown();
            return Task.FromResult(_cache[key]);
        }

        public void Refresh(string key)
        {
            CheckThrown();
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = new())
        {
            CheckThrown();
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            CheckThrown();
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key, CancellationToken token = new())
        {
            CheckThrown();
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            CheckThrown();
            _cache[key] = value;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = new())
        {
            CheckThrown();
            _cache[key] = value;

            return Task.CompletedTask;
        }
    }
}