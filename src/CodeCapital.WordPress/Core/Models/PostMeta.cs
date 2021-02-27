using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_postmeta")]
    public class PostMeta
    {
        [Key]
        [Column("meta_id")]
        public int Id { get; set; }
        [Column("post_id")] public int PostId { get; set; }
        [Column("meta_key")] public string Key { get; set; } = default!;
        [Column("meta_value")] public string Value { get; set; } = default!;

        public Post Post { get; set; } = null!;
    }
}
