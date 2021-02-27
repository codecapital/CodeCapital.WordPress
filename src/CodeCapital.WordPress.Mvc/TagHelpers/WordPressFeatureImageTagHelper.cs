using CodeCapital.WordPress.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace CodeCapital.WordPress.Mvc.TagHelpers
{
    /// <summary>
    /// This will create a regular &lt;img src="" srcset="" &gt; tag including srcset
    /// </summary>
    [HtmlTargetElement("wordpress-featured-img", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class WordPressFeatureImageTagHelper : TagHelper
    {
        [HtmlAttributeName("srcset")] public FeaturedImage FeaturedImage { get; set; } = default!;

        public string Url { get; set; } = "";
        public string Class { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (FeaturedImage == null || !FeaturedImage.HasOriginal) return;

            output.TagName = "img";

            output.Attributes.SetAttribute("loading", "lazy");
            output.Attributes.SetAttribute("srcset", GetSrcSet());
            output.Attributes.SetAttribute("alt", FeaturedImage.Alt);
            output.Attributes.SetAttribute("class", Class);

            // This is fallback, if srcset not recognised. Order is not important.
            if (FeaturedImage.HasMediumLarge)
                output.Attributes.SetAttribute("src", $"{Url}{FeaturedImage.MediumLarge}");
            else if (FeaturedImage.HasLarge)
                output.Attributes.SetAttribute("src", $"{Url}{FeaturedImage.Large}");
            else
                output.Attributes.SetAttribute("src", $"{Url}{FeaturedImage.Original}");

            //output.Content.SetHtmlContent($"<p>Hello, World, Images {FeaturedImage.GetImages().Count}</p>");
        }

        private object GetSrcSet()
        {
            var srcset = new List<string>();

            //if (FeaturedImage.HasOriginal) srcset.Add($"{Url}{FeaturedImage.Original}");
            if (FeaturedImage.HasLarge) srcset.Add($"{Url}{FeaturedImage.Large} 1024w");
            if (FeaturedImage.HasMediumLarge) srcset.Add($"{Url}{FeaturedImage.MediumLarge} 768w");
            if (FeaturedImage.HasMedium) srcset.Add($"{Url}{FeaturedImage.Medium} 300w");
            if (FeaturedImage.HasThumbnail) srcset.Add($"{Url}{FeaturedImage.Thumbnail} 150w");

            return string.Join(",", srcset);
        }
    }
}
