namespace CodeCapital.WordPress.Core.Models
{
    public class PostTerm
    {
        public int PostId { get; set; }

        public Term Term { get; set; } = new Term();

        public PostTerm(int postId, int termId)
        {
            PostId = postId;
            Term.Id = termId;
        }

        //public Term ToTerm()
        //{
        //    return new Term
        //    {
        //        Id = Id,
        //        Name = Name,
        //        Slug = Slug,
        //        Taxonomy = Taxonomy
        //    };
        //}
    }
}
