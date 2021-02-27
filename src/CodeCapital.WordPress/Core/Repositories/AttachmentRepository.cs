using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    // where do we need this?
    // slider library, or simply getting images
    public class AttachmentRepository
    {
        private readonly WordPressDbContext _context;

        public AttachmentRepository(WordPressDbContext context) => _context = context;

        public Task<Post> GetAsync(string url)
            => (from s in _context.Posts
                where s.PostName == url && s.PostType == "attachment"
                select s).AsNoTracking().FirstOrDefaultAsync();

        public Task<List<Post>> GetAsync(List<string> guids)
            => (from p in _context.Posts
                where p.PostType == "attachment" && guids.Contains(p.Guid)
                select p).AsNoTracking().ToListAsync();

        public Task<List<Post>> GetAsync(List<int> ids)
            => (from p in _context.Posts
                where p.PostType == "attachment" && ids.Contains(p.Id)
                select p).AsNoTracking().ToListAsync();

        public virtual async Task<IEnumerable<Post>> GetAsync(SearchQuery searchQuery)
        {
            var posts = from p in _context.Posts
                        where p.PostType == "attachment"
                        select p;

            // Maybe there is a better way of doing this
            foreach (var id in searchQuery.TermIds)
                posts = from p in posts
                        join s in _context.TermRelationships on p.Id equals s.ObjectId
                        where s.TermTaxonomyId == id
                        select p;

            //if (searchQuery.Term?.Id > 0)
            //{
            //    posts = from p in posts
            //            join s in _context.TermRelationships on p.Id equals s.ObjectId
            //            where s.TermTaxonomyId == searchQuery.Term.Id
            //            select p;
            //}

            return await posts.OrderByDescending(o => o.PostDate).AsNoTracking().ToListAsync();
        }
    }
}
