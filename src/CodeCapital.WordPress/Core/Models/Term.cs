using CodeCapital.WordPress.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_term")]
    public class Term
    {
        [Key]
        [Column("term_id")]
        public int Id { get; set; }

        [Column("name")] public string Name { get; set; } = default!;

        [Column("slug")] public string Slug { get; set; } = default!;

        // This is not used anywhere
        //[NotMapped]
        //[Column("term_group")] public long TermGroup { get; set; }

        public TermTaxonomy TermTaxonomy { get; set; } = default!;

        [NotMapped] public string Taxonomy { get; set; } = default!;

        [NotMapped]
        public TaxonomyType Type
        {
            get
            {
                if (Taxonomy == TaxonomyType.Category.GetDescription()) return TaxonomyType.Category;
                if (Taxonomy == TaxonomyType.Tag.GetDescription()) return TaxonomyType.Tag;
                if (Taxonomy == TaxonomyType.Menu.GetDescription()) return TaxonomyType.Menu;

                return TaxonomyType.Unknown;
            }
        }
    }
}
