using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    // Sync this with CachedTermService,
    public class TermService : ITermService
    {
        private readonly ITermRepository _termRepository;

        //protected List<string> _excludeTerms = new List<string>();

        public TermService(ITermRepository termRepository) => _termRepository = termRepository;

        public async Task<Term?> GetAsync(string slug, TaxonomyType type)
        {
            return await _termRepository.GetTerm(slug, type);

            //var terms = await GetAsync();

            //return terms.FirstOrDefault(w => w.Slug == slug && w.Type == type); CSharpFunctionalExtensions
        }

        public async Task<List<PostTerm>> GetListAsync(string slug, TaxonomyType type)
        {
            var terms = await GetPostTermsAsync();

            return terms.Where(w => w.Term.Slug == slug && w.Term.Type == type).ToList();
        }

        public virtual async Task<List<Term>> GetAsync() => await _termRepository.GetAsync();

        public async Task<List<PostTerm>> GetPostTermsAsync(int postId)
            => await GetPostTermsAsync(new List<int> { postId });

        public async Task<List<PostTerm>> GetPostTermsAsync(IEnumerable<int> postIds)
        {
            var postTerms = await GetPostTermsAsync();

            return postTerms.Where(w => postIds.Contains(w.PostId)).ToList();
        }

        public virtual async Task<List<PostTerm>> GetPostTermsAsync()
        {
            var postTerms = await _termRepository.GetPostTermsAsync();

            var terms = await GetAsync();

            postTerms.ForEach(s => s.Term = terms.FirstOrDefault(w => w.Id == s.Term.Id)!);

            return postTerms;
        }
    }
}
