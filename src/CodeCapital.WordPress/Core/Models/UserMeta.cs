using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_usermeta")]
    public class UserMeta
    {
        [Key]
        [Column("umeta_id")]
        public int Id { get; set; }
        [Column("user_id")] public int UserId { get; set; }
        [Column("meta_key")] public string Key { get; set; } = default!;
        [Column("meta_value")] public string Value { get; set; } = default!;
    }
}
