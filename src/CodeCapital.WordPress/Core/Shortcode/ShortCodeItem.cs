using System;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Shortcode
{
    public class ShortCodeItem
    {
        public string Code { get; set; }
        public Func<string, Task<string>> Function { get; set; }
        public bool BroadMatch { get; set; }

        public ShortCodeItem(string code, Func<string, Task<string>> function)
        {
            Code = code;
            Function = function;
        }

        public ShortCodeItem(string code, Func<string, Task<string>> function, bool broadMatch)
        {
            Code = code;
            Function = function;
            BroadMatch = broadMatch;
        }
    }
}
