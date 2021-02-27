using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public class TermRepository : ITermRepository
    {
        private readonly WordPressDbContext _context;

        public TermRepository(WordPressDbContext context) => _context = context;

        public async Task<Term?> GetTerm(string slug, TaxonomyType type)
        {
            var terms = await GetAsync();

            return terms.FirstOrDefault(w => w.Slug == slug && w.Type == type);
        }

        public Task<List<PostTerm>> GetPostTermsAsync()
            => (from p in _context.Posts
                join r in _context.TermRelationships on p.Id equals r.ObjectId
                join tt in _context.TermTaxonomies on r.TermTaxonomyId equals tt.Id
                where p.Status == PostStatusType.Publish
                select new PostTerm(p.Id, tt.TermId))
                .AsNoTracking().ToListAsync();

        //[Obsolete]
        //public async Task<IEnumerable<PostTerm>> GetUsedOnly2Async()
        //{
        //    var sql =
        //        $@"SELECT tt.term_id as Id, p.ID as PostId, Name, Slug, Taxonomy FROM {_context.TablePrefix}posts p
        //        JOIN {_context.TablePrefix}term_relationships s ON p.Id = s.object_id
        //        JOIN {_context.TablePrefix}term_taxonomy tt ON s.term_taxonomy_id = tt.term_taxonomy_id
        //        JOIN {_context.TablePrefix}terms t ON tt.term_id = t.term_id AND p.Status = 'publish'";

        //    var terms = await _context.PostTerm.FromSql(sql).AsNoTracking().ToListAsync();

        //    return terms;
        //}

        //ToDo Maybe transform this to ConcurrentDictionary<>, maybe LEFT JOIN and remove TermTaxonomy
        //SELECT `s`.`name`, `s`.`slug`, `s`.`term_id` AS `Id`, `s.TermTaxonomy`.`Taxonomy`
        //FROM `GsO985_terms` AS `s`
        //LEFT JOIN `GsO985_term_taxonomy` AS `s.TermTaxonomy` ON `s`.`term_id` = `s.TermTaxonomy`.`term_id`
        public Task<List<Term>> GetAsync()
            => _context.Terms.Select(s => new Term
            {
                Name = s.Name,
                Slug = s.Slug,
                Id = s.Id,
                Taxonomy = s.TermTaxonomy.Taxonomy
            }).AsNoTracking().ToListAsync();
    }
}
