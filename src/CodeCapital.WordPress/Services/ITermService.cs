using CodeCapital.WordPress.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    // We will use Lists so we don't have to create copies of it in the memory
    public interface ITermService
    {
        // Probably we don't need these commented and can be removed later
        //Task<IEnumerable<Term>> GetUsedOnlyUniqueAsync();
        //Task<List<Term>> GetAsync(int postId);
        //Task<List<Post>> AddTermsAsync(List<Post> posts);

        Task<Term?> GetAsync(string slug, TaxonomyType type);
        Task<List<Term>> GetAsync();
        //Task<IEnumerable<PostTerm>> GetPostTermsCachedAsync();
        Task<List<PostTerm>> GetPostTermsAsync();
        Task<List<PostTerm>> GetPostTermsAsync(IEnumerable<int> postIds);
        Task<List<PostTerm>> GetPostTermsAsync(int postId);
        Task<List<PostTerm>> GetListAsync(string slug, TaxonomyType type);
    }
}