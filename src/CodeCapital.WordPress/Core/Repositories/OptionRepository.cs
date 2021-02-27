using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CodeCapital.WordPress.Core.Models;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class OptionRepository
    {
        private readonly WordPressDbContext _context;

        public OptionRepository(WordPressDbContext context) => _context = context;

        public async Task<Option> GetAsync(string name)
            => await _context.Options.AsNoTracking().Where(w => w.Name == name).FirstOrDefaultAsync();
    }
}
