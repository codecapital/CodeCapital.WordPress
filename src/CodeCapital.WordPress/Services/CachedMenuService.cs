using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class CachedMenuService : MenuService, ICachedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedMenuService(MenuRepository menuRepository, IMemoryCache memoryCache) : base(menuRepository)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.MinutesToCache));
        }

        // This cannot be used because it counts only with a single menu
        //public async Task<List<MenuItem>> GetMenu2Async(int menuId)
        //    => await _memoryCache.GetOrCreateAsync(Constants.MenuCacheKey + menuId, entry =>
        //                                          {
        //                                              entry.SetOptions(_cacheOptions);

        //                                              return _menuRepository.GetMenuAsync(menuId);
        //                                          });

        public override async Task<List<MenuItem>> GetMenuAsync(int menuId)
        {

            var items = _memoryCache.GetOrCreate(Constants.MenuCacheKey, entry =>
                                                 {
                                                     entry.SetOptions(_cacheOptions);

                                                     return new ConcurrentDictionary<int, Task<List<MenuItem>>>();
                                                 });

            var menu = await items.GetOrAdd(menuId, _ => base.GetMenuAsync(menuId));

            //items.TryGetValue(menuId, out var menu);

            //if (menu != null) return menu;

            //menu = await base.GetMenuAsync(menuId);

            //if (menu == null) return new List<MenuItem>();

            //items.TryAdd(menuId, menu);

            return menu;
        }

        public void ClearCache() => _memoryCache.Remove(Constants.MenuCacheKey);
    }
}
