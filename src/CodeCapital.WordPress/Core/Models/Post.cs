using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_post")]
    public class Post
    {
        private const string FeaturedImageAltMetaKey = "_wp_attachment_image_alt";
        private const string FeaturedImageMetaKey = "_wp_attachment_metadata";
        //private List<Image> _images = new List<Image>();
        private FeaturedImage? _featuredImage;

        [Key] public int Id { get; set; }

        [Column("post_author")] public int AuthorId { get; set; }

        [Column("post_date")] public DateTime PostDate { get; set; }

        [Column("post_date_gmt")] public DateTime PostDateGmt { get; set; }

        [Column("post_content")] public string Content { get; set; } = null!;

        [Column("post_title")] public string Title { get; set; } = null!;

        [Column("post_excerpt")] public string Excerpt { get; set; } = null!;

        [Column("post_status")] public PostStatusType Status { get; set; } = default!;

        //[Column("post_status")] public string Status2 { get; set; } = default!;

        [Column("post_name")] public string PostName { get; set; } = null!;

        [Column("post_modified_gmt")] public DateTime PostModifiedGmt { get; set; }

        public string Guid { get; set; } = null!;

        [Column("post_type")] public string PostType { get; set; } = null!;

        // If this is not used, we can keep it removed for now
        //public int menu_order { get; set; }
        //public int post_parent { get; set; }
        //public DateTime post_modified { get; set; }
        //public string post_content_filtered { get; set; }
        //public string to_ping { get; set; }
        //public string pinged { get; set; }
        //public string comment_status { get; set; }
        //public string ping_status { get; set; }
        //public string post_password { get; set; }
        //public string post_mime_type { get; set; }
        //public long comment_count { get; set; }

        // If this is not used, we should remove it
        //public ICollection<TermRelationship> TermRelationships { get; set; }

        //public Post()
        //{
        //    TermRelationships = new Collection<TermRelationship>();
        //}

        // Additional Details

        [NotMapped] public List<Term> Terms { get; set; } = new List<Term>();

        [NotMapped] public List<PostMeta> PostMeta { get; set; } = new List<PostMeta>();

        [NotMapped] public IEnumerable<Term> Tags => Terms.Where(w => w.Type == TaxonomyType.Tag).Select(s => s);

        [NotMapped] public IEnumerable<Term> Categories => Terms.Where(w => w.Type == TaxonomyType.Category).Select(s => s);

        [NotMapped] public bool IsPublished => Status == PostStatusType.Publish;

        [NotMapped] public string Url { get; set; } = "";

        [NotMapped] public HtmlString HtmlContent { get; set; } = new HtmlString("");

        [NotMapped]
        public FeaturedImage FeaturedImage
        {
            get
            {
                // This could be extracted out of this model is it doesn't belong to here

                //if (_featuredImage == null && HasMeta(FeaturedImageMetaKey))
                //    _featuredImage = new FeaturedImage(ImageNameExtractor.Extract(Meta(FeaturedImageMetaKey)));

                if (_featuredImage == null && HasMeta(FeaturedImageMetaKey))
                    _featuredImage = new FeaturedImage(GetMeta(FeaturedImageMetaKey), Meta(FeaturedImageAltMetaKey) ?? Title);

                return _featuredImage ?? new FeaturedImage();
            }
        }

        //[NotMapped]
        //public List<Image> Images
        //{
        //    get
        //    {
        //        if (!_images.Any())
        //        {
        //            _images = ImageNameExtractor.Extract(Meta(FeaturedImageMetaKey));
        //        }

        //        return _images;
        //    }
        //}

        //[NotMapped] public string FeaturedImageAlt => Meta(FeaturedImageAltMetaKey) ?? Title;

        public string? Meta(string key) => string.IsNullOrWhiteSpace(key) ? null : PostMeta.FirstOrDefault(w => w.Key == key)?.Value;

        public bool HasTag(string slug) => Tags.Any(w => w.Slug == slug);

        public bool HasCategory(string slug) => Categories.Any(w => w.Slug == slug);

        public bool HasMeta(string key) => PostMeta.Any(w => w.Key == key);

        public bool HasTerm(int id) => Terms.Any(a => a.Id == id);

        public bool HasTerms(List<int> termIds) => termIds.All(HasTerm);

        //public bool HasImage(string key) => Images.Any(w => w.Type.GetDescription() == key);

        //[Obsolete("Replaced with GetImage(ImageType type)", true)]
        //public string GetImage(string key) => Images.FirstOrDefault(w => w.Type.GetDescription() == key)?.Name;

        //public string? GetImage(ImageType type) => Images.FirstOrDefault(w => w.Type == type)?.Name;

        public string? GetMeta(string key) => PostMeta.FirstOrDefault(w => w.Key == key)?.Value;

        public bool IsNew(int days) => PostDateGmt.AddDays(days) >= DateTime.Now;
    }
}
