using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.TagHelpers
{
    [HtmlTargetElement("wordpress")]
    public class WordPressTagHelper : TagHelper
    {
        private readonly IPostService _postService;
        public string Url { get; set; } = string.Empty;

        public WordPressTagHelper(IPostService postService) => _postService = postService;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            if (string.IsNullOrWhiteSpace(Url))
            {
                EmptyContent();
                return;
            }

            var post = await _postService.GetAsync(Url);

            if (post == NullPost.Instance)
            {
                EmptyContent();
                return;
            }

            output.Content.SetHtmlContent(post.HtmlContent);

            void EmptyContent() => output.Content.SetHtmlContent(string.Empty);
        }
    }
}
