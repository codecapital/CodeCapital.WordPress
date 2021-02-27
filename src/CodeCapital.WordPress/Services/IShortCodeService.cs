using CodeCapital.WordPress.Core.Models;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public interface IShortCodeService
    {
        Task<string> ProcessAsync(string text);
    }

    public interface IPostProcessingService
    {
        Task ProcessAsync(Post post);
    }

    public class PostProcessingService : IPostProcessingService
    {
        public async Task ProcessAsync(Post post) => await Task.CompletedTask;
    }
}
