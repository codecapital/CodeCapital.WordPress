using System.Collections.Generic;
using System.Threading.Tasks;
using CodeCapital.WordPress.Core.Models;

namespace CodeCapital.WordPress.Services
{
    public interface IMetadataService
    {
        Task<List<PostMeta>> GetAsync(int postId);
        Task<List<PostMeta>> GetAsync();
        Task<List<PostMeta>> GetAsync(IEnumerable<int> postIds);
    }
}