using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Shortcode
{
    public class ShortCodeManager
    {
        /// <summary>
        /// Short code keys and function storage
        /// </summary>
        public ConcurrentDictionary<string, ShortCodeItem> Shortcodes { get; }

        public List<ShortCodeItem> BroadMatchShortcodes => Shortcodes.Where(w => w.Value.BroadMatch).Select(s => s.Value).ToList();

        public bool BroadMatchAvailable => Shortcodes.Any(a => a.Value.BroadMatch);

        public ShortCodeManager() => Shortcodes = new ConcurrentDictionary<string, ShortCodeItem>();

        /// <summary>
        /// Add short code processing function
        /// </summary>
        /// <param name="key">Short code</param>
        /// <param name="function">Function returned to process a code</param>
        /// <param name="broadMatch">Is broad match so the number can be captured e.g. [gallery id=123]</param>
        public void AddFunction(string key, Func<string, Task<string>> function, bool broadMatch = false)
        {
            var fixedKey = ReOrderSequence(key);

            Shortcodes.TryAdd(fixedKey, new ShortCodeItem(fixedKey, function, broadMatch));
        }

        public static string ReOrderSequence(string sequence)
            => string.Join(" ", sequence.ToLower().Replace("[", "").Replace("]", "")
                .Split(' ').OrderBy(o => o));
    }
}
