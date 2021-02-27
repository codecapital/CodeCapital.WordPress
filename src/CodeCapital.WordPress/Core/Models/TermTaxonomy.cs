using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCapital.WordPress.Core.Models
{
    [Table("wp_term_taxonomy")]
    public class TermTaxonomy
    {
        [Key]
        [Column("term_taxonomy_id")]
        public int Id { get; set; }
        [Column("term_id")] public int TermId { get; set; }
        public string Taxonomy { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Parent { get; set; }
        public long Count { get; set; }

        public Term Term { get; set; } = default!;
        public ICollection<TermRelationship> TermRelationships { get; set; }

        public TermTaxonomy()
        {
            TermRelationships = new Collection<TermRelationship>();
        }
    }
}
