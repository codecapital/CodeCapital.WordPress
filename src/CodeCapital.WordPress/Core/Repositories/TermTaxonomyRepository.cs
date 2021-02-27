using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeCapital.WordPress.Core.Models;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class TermTaxonomyRepository
    {
        private readonly WordPressDbContext _context;

        public TermTaxonomyRepository(WordPressDbContext context) => _context = context;

        public Task<TermTaxonomy> GetAsync(string slug) => _context.TermTaxonomies
            .Include(i => i.Term)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Term.Slug == slug);

        public Task<List<TermTaxonomy>> GetAsync() => _context.TermTaxonomies
            .Include(i => i.Term)
            .AsNoTracking().ToListAsync();

        //public Task<List<wp_term_taxonomy>> GetAllByParentIdAsync(int id) => _context.wp_term_taxonomys.Include(i => i.wp_term).Where(w => w.parent == id).AsNoTracking().ToListAsync();
    }
}
