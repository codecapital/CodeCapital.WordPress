using CodeCapital.WordPress.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;

namespace CodeCapital.WordPress.Persistence
{
    public class WordPressDbContext : DbContext
    {
        //public virtual DbSet<CommentMeta> CommentMetas { get; set; } = default!;
        //public virtual DbSet<Comment> Comments { get; set; } = default!;
        //public virtual DbSet<Link> Links { get; set; } = default!;
        public virtual DbSet<Option> Options { get; set; } = default!;
        public virtual DbSet<PostMeta> PostMetas { get; set; } = default!;
        public virtual DbSet<Post> Posts { get; set; } = default!;
        public virtual DbSet<TermRelationship> TermRelationships { get; set; } = default!;
        public virtual DbSet<TermTaxonomy> TermTaxonomies { get; set; } = default!;
        public virtual DbSet<Term> Terms { get; set; } = default!;
        public virtual DbSet<UserMeta> UserMetas { get; set; } = default!;
        public virtual DbSet<User> Users { get; set; } = default!;

        public readonly string TablePrefix = "wp_";

        public WordPressDbContext(DbContextOptions<WordPressDbContext> options, IOptions<WordPressSettings> wordPressSettings) : base(options)
        {
            TablePrefix = wordPressSettings.Value.TablePrefix ?? TablePrefix;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>().HasNoKey();
            modelBuilder.Entity<Option>().ToTable($"{TablePrefix}options");
            modelBuilder.Entity<Post>().ToTable($"{TablePrefix}posts");
            modelBuilder.Entity<PostMeta>().ToTable($"{TablePrefix}postmeta");
            modelBuilder.Entity<Term>().ToTable($"{TablePrefix}terms");
            modelBuilder.Entity<TermTaxonomy>().ToTable($"{TablePrefix}term_taxonomy");
            modelBuilder.Entity<TermRelationship>().ToTable($"{TablePrefix}term_relationships");

            modelBuilder.Entity<TermRelationship>().HasKey(k => new { object_id = k.ObjectId, term_taxonomy_id = k.TermTaxonomyId });

            //modelBuilder.Entity<Post>()
            //    .HasMany(m => m.TermRelationships)
            //    .WithOne(r => r.Post).HasForeignKey(k => k.ObjectId);


            modelBuilder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<PostStatusType>());

            // Disabling this because of performance
            //modelBuilder.Entity<Post>()
            //    .HasMany(m => m.PostMeta)
            //    .WithOne(r => r.Post).HasForeignKey(k => k.PostId);


            //modelBuilder.Entity<TermTaxonomy>()
            //    .HasMany(m => m.TermRelationships)
            //    .WithOne(r => r.TermTaxonomy).HasForeignKey(k => k.TermTaxonomyId);

            //modelBuilder.Entity<TermTaxonomy>()
            //    .HasOne(r => r.Term)
            //    .WithOne(r => r.TermTaxonomy)
            //    .HasForeignKey<Term>();

            //modelBuilder.Entity<PostTerm>().HasKey(k => new { TermId = k.Id, k.PostId });

            //modelBuilder.Entity<wp_post>()
            //    .HasMany(s => s.wp_term_taxonomys)
            //    .WithMany()
            //    .Map(pt =>
            //    {
            //        pt.ToTable("wp_term_relationships");
            //        pt.MapLeftKey("object_id");
            //        pt.MapRightKey("term_taxonomy_id");
            //    });

            //modelBuilder.Entity<wp_post>()
            //    .HasMany(s => s.wp_term_taxonomys)
            //    .WithMany(s => s.wp_posts)
            //    .Map(pt =>
            //    {
            //        pt.ToTable("wp_term_relationships");
            //        pt.MapLeftKey("object_id");
            //        pt.MapRightKey("term_taxonomy_id");
            //    });

        }
    }
}
