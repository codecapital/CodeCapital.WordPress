using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.Services
{
    public class MediaService
    {
        private readonly AttachmentRepository _attachmentRepository;

        public MediaService(AttachmentRepository attachmentRepository) => _attachmentRepository = attachmentRepository;

        public async Task<Dictionary<string, (string title, string description)>> GetDescription(Dictionary<string, (string title, string description)> guids)
        {
            var items = await _attachmentRepository.GetAsync(guids.Keys.ToList());

            foreach (var key in guids.Keys.ToList())
            {
                var post = items.FirstOrDefault(w => w.Guid == key);

                guids[key] = (post?.Title ?? "", post?.Content ?? "");
            }

            return guids;
        }

        public Task<List<Post>> GetAsync(List<int> ids) => _attachmentRepository.GetAsync(ids);
    }
}
