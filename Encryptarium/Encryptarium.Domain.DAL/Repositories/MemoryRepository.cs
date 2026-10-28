using Microsoft.Extensions.Caching.Memory;

namespace Encryptarium.Domain.DAL.Repositories
{
    public interface IMemoryRepository
    {
        void CreateOrUpdate(KeyValuePair<string, string> keyValue);
        string GetAsync(string key);
        void DeleteAsync(string key);
    }

    public class MemoryRepository : IMemoryRepository
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void CreateOrUpdate(KeyValuePair<string, string> keyValue)
        {
            _memoryCache.Set(keyValue.Key, keyValue.Value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.MaxValue)
                .SetSlidingExpiration(TimeSpan.MaxValue)
                .SetPriority(CacheItemPriority.High)
                );
        }

        public void DeleteAsync(string key)
        {
            _memoryCache.Remove(key);
        }

        public string GetAsync(string key)
        {
            return _memoryCache.Get(key) as string;
        }
    }
}
