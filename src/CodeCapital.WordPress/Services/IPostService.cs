using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public interface IPostService
    {
        Task<Post> GetAsync(string url);
        //Task<Post> GetPublishedAsync(string url);
        Task<PaginatedList> GetAsync();
        Task<PaginatedList> GetAsync(SearchQuery searchQuery);
        Task<PaginatedList> GetAsync(IQueryable<Post> query, SearchQuery searchQuery);

        /// <summary>
        /// Use this if you are accessing PaginatedList Posts HtmlContent directly. When you are accessing 
        /// an individual post directly this is already called.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<PaginatedList> AddShortCodeAsync(PaginatedList list);

        Task<IEnumerable<Post>> GetRandomPostsAsync(SearchQuery searchQuery);
        //void ResetCache();
        Task AddMetaAndTermsAndHtmlAsync(Post post);
    }
}