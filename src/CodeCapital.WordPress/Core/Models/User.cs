using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_user")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        //public string user_login { get; set; }
        //public string user_pass { get; set; }
        //public string user_nicename { get; set; }
        [Column("user_email")] public string Email { get; set; } = default!;
        [Column("user_url")] public string Url { get; set; } = default!;
        //public DateTime user_registered { get; set; }
        //public string user_activation_key { get; set; }
        //public int user_status { get; set; }
        [Column("display_name")] public string Name { get; set; } = default!;

        [NotMapped]
        public string? ProfileImageUrl { get; set; }

        [NotMapped]
        public Dictionary<string, string> SocialProfiles { get; set; } = new Dictionary<string, string>();
    }
}
