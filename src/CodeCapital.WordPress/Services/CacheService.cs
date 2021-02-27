using Microsoft.Extensions.Caching.Memory;

namespace CodeCapital.WordPress.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public T Get<T>(string cacheId)
        {
            _memoryCache.TryGetValue(cacheId, out T items);

            return items;
        }
    }
}
