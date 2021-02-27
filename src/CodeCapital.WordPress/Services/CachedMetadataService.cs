using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class CachedMetadataService : MetadataService, ICachedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedMetadataService(MetadataRepository metadataRepository, IMemoryCache memoryCache) : base(metadataRepository)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.MinutesToCache));
        }

        // Most likely won't be used due to performance
        //public async Task<IEnumerable<Metadata>> GetAllCachedAsync()
        //{
        //    _memoryCache.TryGetValue(Constants.MetadataCacheKey, out List<Metadata> metas);

        //    if (metas != null) return metas;

        //    metas = (await _metadataRepository.GetAsync()).ToList();

        //    SaveCache(metas);

        //    return metas;
        //}

        // This is loading all Meta on the first load, while in small blogs it might be ok not when there is 10,000 posts
        // We could cache it gradually if needed (not implemented)
        public override async Task<List<PostMeta>> GetAsync(int postId) =>

            (await GetAsync()).Where(w => w.PostId == postId).ToList();

        public override async Task<List<PostMeta>> GetAsync()
        {
            var metadata = await _memoryCache.GetOrCreateAsync(Constants.MetadataCacheKey, async entry =>
            {
                entry.SetOptions(_cacheOptions);

                return await base.GetAsync();
            });

            return metadata;
        }

        // This is loading all Meta on the first load, while in small blogs it might be ok not when there is 10,000 posts
        // We could cache it gradually if needed (not implemented)
        public override async Task<List<PostMeta>> GetAsync(IEnumerable<int> postIds)
            => (await GetAsync()).Where(w => postIds.Contains(w.PostId)).ToList();

        //public async Task<List<Post>> AddMetadataAsync(List<Post> posts)
        //{
        //    var cachedMeta = await GetAsync();

        //    //var postList = posts.ToList();

        //    //var missingIds = postList.ToList().Select(post => post.Id).Except(cachedMeta.Select(s => s.PostId)).ToList();

        //    //if (missingIds.Count > 0)
        //    //{
        //    //    cachedMeta = AddMissingPostMetaAsync(missingIds).Result.ToList();
        //    //}

        //    return ApplyPostMeta(posts, cachedMeta);
        //}


        //private async Task<IEnumerable<Metadata>> AddMissingPostMetaAsync(List<int> ids)
        //{
        //    var newMetas = (await _metadataRepository.GetAsync(ids)).ToList();

        //    return !newMetas.Any() ? new List<Metadata>() : UpdateCache(newMetas);
        //}

        //private IEnumerable<Metadata> UpdateCache(IEnumerable<Metadata> newMetas)
        //{
        //    MemoryCache.TryGetValue(Constants.MetadataCacheKey, out List<Metadata> metas);

        //    metas = metas ?? new List<Metadata>();

        //    foreach (var newMeta in newMetas)
        //    {
        //        if (metas.Any(a => a.Id == newMeta.Id && a.PostId == newMeta.PostId)) continue;

        //        metas.Add(newMeta);
        //    }

        //    // SaveCache(metas);

        //    return metas;
        //}

        //private void SaveCache(IEnumerable<Metadata> metas)
        //{
        //    _memoryCache.Set(CacheKey, metas, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.MinutesToCache)));
        //}

        //private static List<Post> ApplyPostMeta(List<Post> posts, List<Metadata> postMeta)
        //{
        //    foreach (var post in posts)
        //    {
        //        post.PostMeta = postMeta.Where(t => t.PostId == post.Id).ToList();
        //    }

        //    return posts;
        //}

        public void ClearCache() => _memoryCache.Remove(Constants.MetadataCacheKey);
    }
}
