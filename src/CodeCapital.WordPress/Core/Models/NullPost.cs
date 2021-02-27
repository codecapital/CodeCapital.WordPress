using System;

namespace CodeCapital.WordPress.Core.Models
{
    public sealed class NullPost : Post
    {
        private static readonly Lazy<NullPost> Lazy = new Lazy<NullPost>(() => new NullPost());

        public static NullPost Instance => Lazy.Value;

        private NullPost() => Status = PostStatusType.Inherit;
    }
}
