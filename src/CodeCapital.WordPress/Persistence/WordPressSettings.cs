using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeCapital.WordPress.Persistence
{
    public class WordPressSettings
    {
        /// <summary>
        /// Default prefix wp_
        /// </summary>
        public string TablePrefix { get; set; } = default!;

        /// <summary>
        /// If empty, all will be included
        /// </summary>
        public string AllowedMetaKeys { get; set; } = "";

        public string SqlFormattedAllowedMetaKeys => GetQuotedString(AllowedMetaKeys.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

        public string GetQuotedString(IEnumerable<string> list) => string.Join(",", list.Select(s => $"\"{s.Trim()}\""));
    }
}
