using CodeCapital.WordPress.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CodeCapital.WordPress.Core.Models
{
    public class FeaturedImage
    {
        private readonly List<Image> _images;

        public string Original => Image(ImageType.Original);
        public string Thumbnail => Image(ImageType.Thumbnail);
        public string Medium => Image(ImageType.Medium);
        public string MediumLarge => Image(ImageType.MediumLarge);
        public string Large => Image(ImageType.Large);
        public string PostThumbnail => Image(ImageType.PostThumbnail);

        public bool HasOriginal => HasImage(ImageType.Original);
        public bool HasThumbnail => HasImage(ImageType.Thumbnail);
        public bool HasMedium => HasImage(ImageType.Medium);
        public bool HasMediumLarge => HasImage(ImageType.MediumLarge);
        public bool HasLarge => HasImage(ImageType.Large);
        public bool HasPostThumbnail => HasImage(ImageType.PostThumbnail);
        public string Alt { get; set; }

        private string Image(ImageType type) => _images.FirstOrDefault(w => w.Type == type)?.Name ?? string.Empty;

        private bool HasImage(ImageType type) => _images.Any(w => w.Type == type);

        //public bool HasImage(string key) => _images.Any(w => w.Type.GetDescription() == key);


        public FeaturedImage(string? serialisedString = null, string imageTitle = "")
        {
            _images = ImageNameExtractor.Extract(serialisedString) ?? new List<Image>();

            Alt = imageTitle;
        }

        public List<Image> GetImages() => _images;

        //public FeaturedImage(List<Image>? images = null)
        //{


        //    _images = images ?? new List<Image>();
        //}
    }
}