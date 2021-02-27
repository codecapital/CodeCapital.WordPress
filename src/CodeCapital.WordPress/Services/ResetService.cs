using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class ResetService
    {
        private readonly IMetadataService _metadataService;
        private readonly IPostService _postService;
        private readonly ITermService _termService;
        private readonly IMenuService _menuService;
        private readonly IMemoryCache _memoryCache;
        private readonly ITermRepository _termRepository;
        private readonly IPostRepository _postRepository;

        public ResetService(IMetadataService metadataService,
            IPostService postService,
            ITermService termService,
            IMenuService menuService,
            IMemoryCache memoryCache,
            ITermRepository termRepository,
            IPostRepository postRepository)
        {
            _metadataService = metadataService;
            _postService = postService;
            _termService = termService;
            _menuService = menuService;
            _memoryCache = memoryCache;
            _termRepository = termRepository;
            _postRepository = postRepository;
        }

        //public async Task ResetAllAsync()
        //{
        //    // Metas and Terms are reset when opening Reset page
        //    //await _unitOfWork.MetadataService.ResetCache();
        //    //await _unitOfWork.TermService.ResetCache();
        //    //_unitOfWork.PageService.ResetCache();

        //    _unitOfWork.PostService.ResetCache();
        //    await _backgroundImageService.ResetBackgroundAsync();

        //    _unitOfWork.AppSettings.GameDetails.Clear();
        //}

        public async Task ResetLatest()
        {
            ((ICachedService)_menuService).ClearCache();
            ((ICachedService)_metadataService).ClearCache();
            await SyncTermsAsync();
            await SyncPostsAsync();
        }

        public void ClearCache()
        {
            ((ICachedService)_menuService).ClearCache();
            ((ICachedService)_termService).ClearCache();
        }

        public async Task SyncPostsAsync()
        {
            //var updatedPosts = await _unitOfWork.PostService.CacheAll(AddUrlAndAltertnateTitle, modifiedPosts);

            var posts = (await _postRepository.GetLastModifiedAsync(new SearchQuery())).ToList();

            foreach (var post in posts)
            {
                var cachedPost = await _postService.GetAsync(post.PostName);

                // Maybe add to cache because we already have it / remove from cache 
                if (cachedPost == NullPost.Instance) return;

                if (post.PostModifiedGmt == cachedPost.PostModifiedGmt) continue;

                await _postService.AddMetaAndTermsAndHtmlAsync(post);

                cachedPost.Title = post.Title;
                cachedPost.Excerpt = post.Excerpt;
                cachedPost.PostDateGmt = post.PostDateGmt;
                cachedPost.HtmlContent = post.HtmlContent;
                cachedPost.Status = post.Status;
            }
        }

        public async Task SyncTermsAsync()
        {
            var cachedTerms = await _termService.GetAsync();

            var terms = await _termRepository.GetAsync();

            foreach (var term in terms)
            {
                var cachedTerm = cachedTerms.FirstOrDefault(w => w.Id == term.Id);

                if (cachedTerm == null)
                {
                    cachedTerms.Add(term);
                    continue;
                }

                cachedTerm.Name = term.Name;
                cachedTerm.Slug = term.Slug;
            }
        }
    }
}
