using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CodeCapital.WordPress.Core.Models;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class UserRepository
    {
        private readonly WordPressDbContext _context;

        public UserRepository(WordPressDbContext context) => _context = context;

        public async Task<User?> GetAuthorAsync(decimal id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if (user == null) return null;

            var metas = await _context.UserMetas.Where(w => w.Id == id).AsNoTracking().ToListAsync();

            if (metas.Count == 0) return user;

            user.SocialProfiles = metas.Distinct().ToDictionary(i => i.Key, i => i.Value);

            return user;

            //foreach (var meta in metas)
            //{
            //    if (!author.SocialProfiles.ContainsKey(meta.meta_key))
            //        author.SocialProfiles.Add(meta.meta_key, meta.meta_value);
            //}
        }
    }
}
