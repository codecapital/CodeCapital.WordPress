using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Extensions;
using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ITermService _termService;
        private readonly IShortCodeService _shortCode;
        private readonly IPostProcessingService _postProcessingService;
        private readonly IMetadataService _metadataService;

        public delegate void PostProcessing(Post post);

        public PostService(
            IPostRepository postRepository,
            ITermService termService,
            IShortCodeService shortCode,
            IPostProcessingService postProcessingService,
            IMetadataService metadataService)
        {
            _postRepository = postRepository;
            _termService = termService;
            _shortCode = shortCode;
            _postProcessingService = postProcessingService;
            _metadataService = metadataService;
        }

        public Task<PaginatedList> GetAsync() => GetAsync(new SearchQuery());

        public async Task<PaginatedList> GetAsync(IQueryable<Post> query, SearchQuery searchQuery)
        {
            var paginatedList = await _postRepository.GetAsync(query, searchQuery);

            await AddAditionalEntities(paginatedList);

            return paginatedList;
        }

        public async Task<PaginatedList> GetAsync(SearchQuery searchQuery)
        {
            var paginatedList = await _postRepository.GetPublishedAsync(searchQuery);

            await AddAditionalEntities(paginatedList);

            return paginatedList;
        }

        public async Task<PaginatedList> AddShortCodeAsync(PaginatedList list)
        {
            await list.Items.AddShortCodeAsync(_shortCode.ProcessAsync);

            return list;
        }

        //[Obsolete("Do we need this?", true)]
        //public async Task<Post> GetPublishedAsync(string url)
        //{
        //    var post = await _postRepository.GetAsync(url);

        //    if (post == null) return null;

        //    // check if this is needed
        //    await AddMetaAndTermsAndHtmlAsync(post);

        //    return post;
        //}

        public virtual async Task<Post> GetAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return NullPost.Instance;

            var post = await _postRepository.GetAsync(url);

            if (post == NullPost.Instance) return post;

            // check if this is needed
            await AddMetaAndTermsAndHtmlAsync(post);

            return post;
        }

        //protected async Task<List<Post>> CacheAll(PostProcessing postProcessing, SearchQuery searchQuery, List<Post> posts = null)
        //{
        //    posts = posts ?? (await _postRepository.GetPublishedPostsAndPagesAsync(searchQuery)).ToList();

        //    var terms = await _termService.GetPostTermsCachedAsync();
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

        public async Task AddMetaAndTermsAndHtmlAsync(Post post)
        {
            await post.AddShortCodeAsync(_shortCode.ProcessAsync);
            await post.AddTermsAsync(_termService);
            await post.AddMetadataAsync(_metadataService);
            await post.AddPostProcessingAsync(_postProcessingService.ProcessAsync);

            //post.PostMeta = await _metadataService.GetAsync(post.Id);
        }

        private async Task AddAditionalEntities(PaginatedList paginatedList)
        {
            await paginatedList.Items.AddTermsAsync(_termService);
            await paginatedList.Items.AddMetadataAsync(_metadataService);
            await paginatedList.Items.AddPostProcessingAsync(_postProcessingService.ProcessAsync);
        }

        //protected async Task ApplyShortCode(Post post) =>
        //    post.Html = PostExtensions.ToHtml(await ShortCodeProcessingAsync(post.Content));

        //protected virtual async Task<string> ShortCodeProcessingAsync(string text) => await Task.Run(() => text);

        private static List<Post> GetRandom(IEnumerable<Post> posts, int count) => posts.OrderBy(x => Guid.NewGuid()).Take(count).ToList();

        //public virtual async Task<IEnumerable<Post>> GetFeaturedPostsAsync()
        //{
        //    ConcurrentDictionary<string, Post> postsDictionary;

        //    _memoryCache.TryGetValue(WordPressPostsKey, out postsDictionary);

        //    if (postsDictionary == null) return new List<Post>();

        //    var featuredPosts = postsDictionary.Where(w => w.Value.IsFeatured).Select(s => s.Value).ToList();

        //    if (featuredPosts.Any()) return featuredPosts;

        //    var featuredTerm = await _termService.GetTermAsync("featured", TaxonomyType.Tag);

        //    if (featuredTerm == null) return new List<Post>();

        //    var posts = (await GetAsync(new Query
        //    {
        //        Term = featuredTerm
        //    })).ToList();

        //    foreach (var post in posts)
        //    {
        //        postsDictionary.TryAdd(post.PostName, post);
        //    }

        //    SaveToCache(postsDictionary);

        //    return posts;
        //}

        public virtual async Task<IEnumerable<Post>> GetRandomPostsAsync(SearchQuery searchQuery) => await Task.Run(() => new List<Post>());
    }
}
