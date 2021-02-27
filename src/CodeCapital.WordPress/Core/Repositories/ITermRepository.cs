using CodeCapital.WordPress.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Repositories
{
    public interface ITermRepository
    {
        Task<List<PostTerm>> GetPostTermsAsync();
        Task<List<Term>> GetAsync();
        Task<Term?> GetTerm(string slug, TaxonomyType type);
    }
}