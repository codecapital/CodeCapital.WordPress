using System;
using System.Collections.Generic;

namespace CodeCapital.WordPress.Core.Models
{
    public class SearchQuery
    {
        /// <summary>
        /// First page is 1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Set to 0 to return all pages
        /// </summary>
        public int PageSize { get; set; } = 10;

        //public int Count { get; set; }

        //public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public string SearchKeyword { get; set; } = "";

        [Obsolete("Can we make this obsolete and replace with TermIds?", true)]
        public Term Term { get; set; } = default!;

        public List<int> TermIds { get; set; }

        public List<string> ExcludeSlugs { get; set; }

        public SearchQuery(int pageIndex = 1, int pageSize = 10) : this()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public SearchQuery()
        {
            TermIds = new List<int>();
            ExcludeSlugs = new List<string>();
        }

        public static SearchQuery CreateWithTerms(params int[] termIds)
        {
            var query = new SearchQuery();

            query.TermIds.AddRange(termIds);

            return query;
        }
    }
}
