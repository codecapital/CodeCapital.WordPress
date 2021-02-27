using CodeCapital.WordPress.Core;
using System;

namespace CodeCapital.WordPress.Mvc.ViewModels
{
    [Obsolete("Retired", true)]
    public class PostsViewModel
    {
        public PaginatedList PaginatedList { get; set; }

        [Obsolete("Retired", true)]
        public string Description { get; set; } = "";

        [Obsolete("Retired", true)]
        public string Title { get; set; } = "";

        [Obsolete("Retired", true)]
        public int CurrentPage { get; set; }

        [Obsolete("Retired", true)]
        public bool IsPager { get; set; }

        [Obsolete("Retired", true)]
        public string NextPageUrl { get; set; } = "";


        [Obsolete("Retired", true)]
        public string Taxonomy { get; set; } = "";

        [Obsolete("Retired", true)]
        public string Slug { get; set; } = "";

        public PostsViewModel() => PaginatedList = new PaginatedList();
    }
}
