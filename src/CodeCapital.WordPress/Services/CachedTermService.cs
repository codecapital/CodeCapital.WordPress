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
    public class CachedTermService : TermService, ICachedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedTermService(ITermRepository termRepository, IMemoryCache memoryCache) : base(termRepository)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.MinutesToCache));
        }

        // Is this used? If yes, move to TermService
        [Obsolete]
        public async Task<List<PostTerm>> GetList2Async(int termId)
        {
            var terms = await GetPostTermsAsync();

            return terms.Where(w => w.Term.Id == termId).ToList();
        }

        public override async Task<List<Term>> GetAsync()
        {
            var terms = await _memoryCache.GetOrCreateAsync(Constants.TermCacheKey, async entry =>
                {
                    entry.SetOptions(_cacheOptions);

                    return await base.GetAsync();
                });

            //terms.RemoveAll(w => _excludeTerms.Contains(w.Slug));

            return terms;
        }

        // This probably doesn't make sense and can be replaced by GetAsync
        //public async Task<Term> GetUsedOnlyTermAsync(string slug, TaxonomyType type)
        //{
        //    var terms = await GetUsedOnlyUniqueAsync();

        //    return terms.FirstOrDefault(w => w.Slug == slug && w.Type == type);
        //}

        //public async Task<IEnumerable<Term>> GetUsedOnlyUniqueAsync()
        //{
        //    var postTerms = await GetPostTermsCachedAsync();

        //    var terms = postTerms.GroupBy(g => new { g.Taxonomy, g.Slug, g.Name }).Select(s => s.First()).OrderBy(o => o.Name);

        //    return terms;
        //}

        // This is used, probably rename GetByPostId
        //public async Task<List<Term>> GetAsync(int postId)
        //{
        //    var postTerms = await GetPostTermsCachedAsync();

        //    return postTerms.Where(w => w.PostId == postId).Select(s => s.Term).ToList();
        //}

        public override async Task<List<PostTerm>> GetPostTermsAsync()
        {
            var postTerms = await _memoryCache.GetOrCreateAsync(Constants.PostTermCacheKey, async entry =>
            {
                entry.SetOptions(_cacheOptions);

                return await base.GetPostTermsAsync();
            });

            //terms.RemoveAll(w => _excludeTerms.Contains(w.Slug));

            return postTerms;
        }

        // Move outside, maybe to Extension
        //protected static List<Post> ApplyTerms(List<Post> posts, List<PostTerm> terms)
        //{
        //    foreach (var post in posts)
        //    {
        //        post.Terms = terms.Where(p => p.PostId == post.Id).Select(t => t.ToTerm()).ToList();
        //    }

        //    return posts;
        //}

        public void ClearCache()
        {
            _memoryCache.Remove(Constants.PostTermCacheKey);
            _memoryCache.Remove(Constants.TermCacheKey);

            //await GetPostTermsAsync();
        }
    }
}
