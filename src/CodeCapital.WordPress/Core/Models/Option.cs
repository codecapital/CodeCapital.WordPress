using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_option")]
    public class Option
    {
        [Key]
        [Column("option_id")]
        public int Id { get; set; }

        [Column("option_name")] public string Name { get; set; } = default!;
        [Column("option_value")] public string Value { get; set; } = default!;
        public string Autoload { get; set; } = default!;
    }
}
