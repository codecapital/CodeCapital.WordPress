using System.ComponentModel;

namespace CodeCapital.WordPress.Core.Models
{
    /// <summary>
    /// Sizes: thumbnail max 150px, medium max 300px, medium_large max 768px, large max 1024px, post-thumbnail max 825px.
    /// </summary>
    public enum ImageType
    {
        [Description("original")]
        Original = 1,

        [Description("thumbnail")]
        Thumbnail = 2,

        [Description("medium")]
        Medium = 3,

        [Description("medium_large")]
        MediumLarge = 4,

        [Description("large")]
        Large = 5,

        [Description("post-thumbnail")]
        PostThumbnail = 6
    }
}
