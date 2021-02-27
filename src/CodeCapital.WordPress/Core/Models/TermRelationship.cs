using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_term_relationship")]
    public class TermRelationship
    {
        [Column("object_id")] public int ObjectId { get; set; }
        [Column("term_taxonomy_id")] public int TermTaxonomyId { get; set; }
        [Column("term_order")] public int TermOrder { get; set; }

        public Post Post { get; set; } = default!;
        public TermTaxonomy TermTaxonomy { get; set; } = default!;
    }
}
