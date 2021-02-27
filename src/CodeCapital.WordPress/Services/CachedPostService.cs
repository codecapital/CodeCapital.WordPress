using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class CachedPostService : PostService, ICachedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        //public delegate void PostProcessing(Post post);

        public CachedPostService(
            IPostRepository postRepository,
            ITermService termService,
            IShortCodeService shortCode,
            IPostProcessingService postProcessingService,
            IMetadataService metadataService,
            IMemoryCache memoryCache) : base(postRepository, termService, shortCode, postProcessingService, metadataService)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.MinutesToCache));
        }

        public override async Task<Post> GetAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return NullPost.Instance;

            var posts = GetMemoryCache();

            var post = await posts.GetOrAdd(url, _ => base.GetAsync(url));

            //posts.TryGetValue(url, out var post);

            //if (post != null) return post;

            //post = await base.GetAsync(url);

            //posts.TryAdd(url, post);

            return post;
        }

        private ConcurrentDictionary<string, Task<Post>> GetMemoryCache()
            => _memoryCache.GetOrCreate(Constants.PostCacheKey, entry =>
            {
                entry.SetOptions(_cacheOptions);

                return new ConcurrentDictionary<string, Task<Post>>();
            });

        //This probably make sense in small blogs but should be implemented better
        //protected async Task<List<Post>> CacheAll(PostProcessing postProcessing, SearchQuery searchQuery, List<Post> posts = null)
        //{
        //    posts = posts ?? (await _postRepository.GetPublishedPostsAndPagesAsync(searchQuery)).ToList();

        //    var terms = await _termService.GetPostTermsAsync();
        //    var metas = await _metadataService.GetAsync();

        //    foreach (var post in posts)
        //    {
        //        post.Terms = GetTerms(post.Id);
        //        post.PostMeta = GetMetas(post.Id);
        //        postProcessing(post);
        //    }

        //    return posts;

        //    List<Term> GetTerms(int postId) => terms.Where(w => w.PostId == postId).Select(s => s.ToTerm()).ToList();

        //    List<Metadata> GetMetas(int postId) => metas.Where(w => w.PostId == postId).ToList();
        //}

        private static List<Post> GetRandom(IEnumerable<Post> posts, int count) => posts.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

        public override async Task<IEnumerable<Post>> GetRandomPostsAsync(SearchQuery searchQuery)
        {
            _memoryCache.TryGetValue(Constants.PostCacheKey, out ConcurrentDictionary<string, Task<Post>> taskPosts);

            if (taskPosts == null) return new List<Post>();

            var posts = (await Task.WhenAll(taskPosts.Select(s => s.Value))).ToList();

            if (searchQuery.TermIds.Any())
                return posts.OrderBy(x => Guid.NewGuid())
                    .Where(w => w.HasTerms(searchQuery.TermIds))
                    .Take(searchQuery.PageSize).ToList();

            return posts.OrderBy(x => Guid.NewGuid()).Take(searchQuery.PageSize).ToList();

            //if (!string.IsNullOrWhiteSpace(searchQuery.Term?.Slug))
            //{
            //    return await Task.Run(() =>
            //    posts.OrderBy(x => Guid.NewGuid())
            //    .Where(w => w.Value.HasTag(searchQuery.Term.Slug))
            //    .Take(searchQuery.PageSize)
            //    .Select(s => s.Value).ToList()
            //    );
            //}

            //return await Task.Run(() =>
            //    posts.OrderBy(x => Guid.NewGuid())
            //    .Where(w => !searchQuery.ExcludeSlugs.Any(s => w.Value.HasTag(s)))
            //    .Take(searchQuery.PageSize)
            //    .Select(s => s.Value).ToList()
            //);
        }

        public void ClearCache() => _memoryCache.Remove(Constants.PostCacheKey);
    }
}
