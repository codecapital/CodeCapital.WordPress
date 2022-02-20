using CodeCapital.WordPress.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public interface IPostRepository
    {
        Task<PaginatedList> GetPublishedAsync(SearchQuery searchQuery);
        Task<PaginatedList> GetAsync(IQueryable<Post> query, SearchQuery searchQuery);
        Task<IEnumerable<Post>> GetPublishedPostsAndPagesAsync(SearchQuery searchQuery);

        // Remove after SierraHigh
        [Obsolete("Replaced with GetLastModifiedAsync", true)]
        Task<IEnumerable<Post>> GetPublishedPostsAndPagesLastModifiedAsync(SearchQuery searchQuery);

        Task<IEnumerable<Post>> GetLastModifiedAsync(SearchQuery searchQuery);
        Task<Post> GetAsync(string url);
        Task<Post> GetPublishedAsync(string url);
    }
}