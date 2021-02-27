using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly MetadataRepository _metadataRepository;

        public MetadataService(MetadataRepository metadataRepository) => _metadataRepository = metadataRepository;

        public virtual Task<List<PostMeta>> GetAsync(int postId)
            => _metadataRepository.GetAsync(postId);

        public virtual Task<List<PostMeta>> GetAsync()
            => _metadataRepository.GetAsync();

        public virtual Task<List<PostMeta>> GetAsync(IEnumerable<int> postIds)
            => _metadataRepository.GetAsync(postIds.ToList());

        //public async Task<List<Post>> AddMetadataAsync(List<Post> posts)
        //{
        //    var cachedMeta = (await GetAsync()).ToList();

        //    //var postList = posts.ToList();

        //    //var missingIds = postList.ToList().Select(post => post.Id).Except(cachedMeta.Select(s => s.PostId)).ToList();

        //    //if (missingIds.Count > 0)
        //    //{
        //    //    cachedMeta = AddMissingPostMetaAsync(missingIds).Result.ToList();
        //    //}

        //    return ApplyPostMeta(posts, cachedMeta);
        //}

        //private async Task<IEnumerable<Metadata>> AddMissingPostMetaAsync(List<int> ids)
        //{
        //    var newMetas = (await _metadataRepository.GetAsync(ids)).ToList();

        //    return !newMetas.Any() ? new List<Metadata>() : UpdateCache(newMetas);
        //}

        //private static List<Post> ApplyPostMeta(List<Post> posts, List<Metadata> postMeta)
        //{
        //    foreach (var post in posts)
        //    {
        //        post.PostMeta = postMeta.Where(t => t.PostId == post.Id).ToList();
        //    }

        //    return posts;
        //}
    }
}
