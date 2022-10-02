using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly WordPressDbContext _context;
        private readonly string[] _contentTypes = { "post", "page" };

        //private const string PublishPostStatus = "publish";

        public PostRepository(WordPressDbContext context) => _context = context;

        public async Task<Post> GetAsync(string url)
        {
            var post = await (from s in _context.Posts
                              where s.PostName == url && _contentTypes.Contains(s.PostType)
                              select s).AsNoTracking().FirstOrDefaultAsync();

            return post ?? NullPost.Instance;
        }

        public async Task<Post> GetPublishedAsync(string url)
        {
            var post = await (from s in _context.Posts
                              where s.PostName == url && _contentTypes.Contains(s.PostType) && s.Status == PostStatusType.Publish
                              select s).AsNoTracking().FirstOrDefaultAsync();

            return post ?? NullPost.Instance;
        }

        public virtual async Task<IEnumerable<Post>> GetLastModifiedAsync(SearchQuery searchQuery)
        {
            var query = from p in _context.Posts
                        where _contentTypes.Contains(p.PostType)
                        select p;

            var postList = await query
                .OrderByDescending(o => o.PostModifiedGmt)
                .Skip(searchQuery.PageSize * (searchQuery.PageIndex - 1))
                .Take(searchQuery.PageSize).AsNoTracking().ToListAsync();

            var posts = postList;

            return posts;
        }

        [Obsolete("Replaced with GetLastModifiedAsync", true)]
        public virtual async Task<IEnumerable<Post>> GetPublishedPostsAndPagesLastModifiedAsync(SearchQuery searchQuery)
        {
            var query = from p in _context.Posts
                        where p.Status == PostStatusType.Publish && _contentTypes.Contains(p.PostType)
                        select p;

            var postList = await query.OrderByDescending(o => o.PostModifiedGmt).Skip(searchQuery.PageSize * (searchQuery.PageIndex - 1)).Take(searchQuery.PageSize).AsNoTracking().ToListAsync();

            var posts = postList;

            return posts;
        }

        public virtual async Task<IEnumerable<Post>> GetPublishedPostsAndPagesAsync(SearchQuery searchQuery)
        {
            var query = from p in _context.Posts
                        where p.Status == PostStatusType.Publish && _contentTypes.Contains(p.PostType)
                        select p;

            var paginatedList = await GetAsync(query, searchQuery);

            return paginatedList.Items;
        }

        public virtual async Task<PaginatedList> GetPublishedAsync(SearchQuery searchQuery)
        {
            var query = from p in _context.Posts
                        where p.Status == PostStatusType.Publish && p.PostType == "post"
                        select p;

            return await GetAsync(query, searchQuery);

        }

        public virtual async Task<PaginatedList> GetAsync(IQueryable<Post> query, SearchQuery searchQuery)
        {

            // Maybe there is a better way of doing this
            if (searchQuery.TermIds.Count > 0)
                query = from p in query
                        join s in _context.TermRelationships on p.Id equals s.ObjectId
                        where searchQuery.TermIds.Contains(s.TermTaxonomyId)
                        select p;

            //foreach (var id in searchQuery.TermIds)
            //    query = from p in query
            //            join s in _context.TermRelationships on p.Id equals s.ObjectId
            //            where s.TermTaxonomyId == id
            //            select p;

            //if (searchQuery.Term.Id > 0)
            //{
            //    query = from p in query
            //            join s in _context.TermRelationships on p.Id equals s.ObjectId
            //            where s.TermTaxonomyId == searchQuery.Term.Id
            //            select p;
            //}

            if (!string.IsNullOrWhiteSpace(searchQuery.SearchKeyword))
            {
                query = from p in query
                        where p.Title.ToLower().Contains(searchQuery.SearchKeyword.ToLower())
                        select p;
            }

            // This might be questionable if there is many ids
            if (searchQuery.ExcludeSlugs.Any())
            {
                var ids = (from p in _context.Posts
                           join s in _context.TermRelationships on p.Id equals s.ObjectId
                           join t in _context.Terms on s.TermTaxonomyId equals t.Id
                           where searchQuery.ExcludeSlugs.Contains(t.Slug)
                           select p.Id).Distinct();

                query = from p in query
                        where !ids.Contains(p.Id)
                        select p;
            }

            var paginatedList = await PaginatedList.CreateAsync(query.OrderByDescending(o => o.PostDate).AsNoTracking(), searchQuery.PageIndex, searchQuery.PageSize);

            //if (searchQuery.PageSize == 0)
            //{
            //    return (await postQueryable.OrderByDescending(o => o.PostDateGmt).AsNoTracking().ToListAsync()).ToPosts();
            //}

            //var postList = await postQueryable.OrderByDescending(o => o.PostDateGmt).Skip(searchQuery.PageSize * searchQuery.PageIndex).Take(searchQuery.PageSize).AsNoTracking().ToListAsync();

            //var posts = postList?.ToPosts();

            return paginatedList;
        }
    }
}
