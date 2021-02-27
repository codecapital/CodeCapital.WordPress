using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Services;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core.Extensions
{
    public static class PostExtensions
    {
        /// <summary>
        /// This a conversion of WordPress php wpautop into c#
        /// https://core.trac.wordpress.org/browser/tags/5.1/src/wp-includes/formatting.php
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HtmlString ToHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new HtmlString(string.Empty);

            text = text.Trim() + "\n";

            text = Regex.Replace(text, @"<br />\s*<br />", "\n\n");

            const string allblocks = "(?:table|thead|tfoot|caption|col|colgroup|tbody|tr|td|th|div|dl|dd|dt|ul|ol|li|pre|form|map|area|blockquote|address|math|style|p|h[1-6]|hr|fieldset|legend|section|article|aside|hgroup|header|footer|nav|figure|figcaption|details|menu|summary)";

            text = Regex.Replace(text, "(<" + allblocks + "[^>]*>)", "\n$1");
            text = Regex.Replace(text, "(</" + allblocks + ">)", "$1\n\n");
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");

            text = Regex.Replace(text, "\n\n+", "\n\n");

            var pees = Regex.Split(text, @"\n\s*\n", RegexOptions.IgnorePatternWhitespace);
            text = "";

            foreach (var item in pees) text += "<p>" + item.Trim('\n') + "</p>\n";

            // under certain strange conditions it could create a P of entirely white space
            text = Regex.Replace(text, @"<p>\s*</p>", "");
            text = Regex.Replace(text, "<p>([^<]+)</(div|address|form)>", "<p>$1</p></$2>");
            // don't pee all over a tag
            text = Regex.Replace(text, @"<p>\s*(</?" + allblocks + @"[^>]*>)\s*</p>", "$1");

            //$pee = preg_replace("|<p>(<li.+?)</p>|", "$1", $pee); // problem with nested lists
            text = Regex.Replace(text, @"<p>(<li.+?)</p>", "$1");
            //$pee = preg_replace('|<p><blockquote([^>]*)>|i', "<blockquote$1><p>", $pee);
            //$pee = str_replace('</blockquote></p>', '</p></blockquote>', $pee);

            text = Regex.Replace(text, @"<p>\s*(</?" + allblocks + "[^>]*>)", "$1");
            text = Regex.Replace(text, "(</?" + allblocks + @"[^>]*>)\s*</p>", "$1");

            text = Regex.Replace(text, @"(?<!<br />)\s*\n", "<br />\n");

            // If a <br /> tag is after an opening or closing block tag, remove it.
            text = Regex.Replace(text, "(</?" + allblocks + @"[^>]*>)\s*<br />", "$1");

            // If a <br /> tag is before a subset of opening or closing block tags, remove it.
            text = Regex.Replace(text, @"<br />(\s*</?(?:p|li|div|dl|dd|dt|th|pre|td|ul|ol)[^>]*>)", "$1");
            text = Regex.Replace(text, "\n</p>$", "</p>");

            return new HtmlString(text);
        }

        public static Task AddPostProcessingAsync(this Post post, Func<Post, Task> processAsync)
            => processAsync(post);

        public static async Task AddPostProcessingAsync(this List<Post> posts, Func<Post, Task> processAsync)
        {
            var tasks = posts.Select(p => p.AddPostProcessingAsync(processAsync));

            await Task.WhenAll(tasks);

            //foreach (var post in posts)
            //{
            //    await post.AddPostProcessingAsync(postProcessingService.ProcessAsync);
            //}
        }

        public static async Task AddShortCodeAsync(this Post post, Func<string, Task<string>> shortCodeProcessing)
            => post.HtmlContent = ToHtml(await shortCodeProcessing(post.Content));

        public static async Task AddShortCodeAsync(this List<Post> posts, Func<string, Task<string>> shortCodeProcessing)
        {
            foreach (var post in posts)
                post.HtmlContent = ToHtml(await shortCodeProcessing(post.Content));

        }

        public static async Task AddMetadataAsync(this Post post, IMetadataService metadataService)
            => post.PostMeta = await metadataService.GetAsync(post.Id);

        public static async Task AddMetadataAsync(this List<Post> posts, IMetadataService metadataService)
        {
            var meta = await metadataService.GetAsync(posts.Select(s => s.Id));

            posts.ForEach(p => p.PostMeta = meta.Where(w => w.PostId == p.Id).ToList());

            //foreach (var post in posts)
            //{
            //    post.PostMeta = meta.Where(w => w.PostId == post.Id).ToList();
            //}
        }

        public static async Task AddTermsAsync(this Post post, ITermService termService)
        {
            //post.Terms = await termService.GetAsync(post.Id);
            var postTerms = await termService.GetPostTermsAsync(post.Id);

            post.Terms = postTerms.Select(s => s.Term).ToList();
        }

        public static async Task AddTermsAsync(this List<Post> posts, ITermService termService)
        {
            var postTerms = (await termService.GetPostTermsAsync(posts.Select(s => s.Id))).ToList();

            posts.ForEach(p => p.Terms = postTerms.Where(w => w.PostId == p.Id).Select(s => s.Term).ToList());

            //foreach (var post in posts)
            //{
            //    post.Terms = postTerms.Where(w => w.PostId == post.Id).Select(s => s.Term).ToList();
            //}

            //return await termService.AddTermsAsync(posts);
        }
    }
}
