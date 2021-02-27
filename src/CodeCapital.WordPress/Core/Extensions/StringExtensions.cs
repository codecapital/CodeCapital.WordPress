using System;
using System.Text;

namespace CodeCapital.WordPress.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Base64ForUrlDecode(this string str)
        {
            var data = Convert.FromBase64String(str);

            return Encoding.UTF8.GetString(data);
        }
    }
}
