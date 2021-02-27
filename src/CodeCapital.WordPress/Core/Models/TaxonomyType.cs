using System.ComponentModel;

namespace CodeCapital.WordPress.Core.Models
{
    public enum TaxonomyType
    {
        Unknown = 0,

        [Description("category")]
        Category = 1,

        [Description("post_tag")]
        Tag = 2,

        [Description("nav_menu")]
        Menu = 3
    }
}
