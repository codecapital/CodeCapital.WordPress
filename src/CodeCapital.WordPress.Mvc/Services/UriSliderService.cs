using CodeCapital.AspNetCore.Mvc.Renderer;
using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Extensions;
using CodeCapital.WordPress.Core.Repositories;
using CodeCapital.WordPress.Mvc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.Services
{
    /// <summary>
    /// Ultimate Responsive Image Slider
    /// </summary>
    public class UriSliderService
    {
        private readonly RazorViewRenderer _renderer;
        private readonly MetadataRepository _metadataRepository;
        private readonly MediaService _mediaService;
        private const string WordPressUrSlider = "uris";
        private const string GalleryMetaKey = "ris_all_photos_details";

        public UriSliderService(RazorViewRenderer renderer, MetadataRepository metadataRepository, MediaService mediaService)
        {
            _renderer = renderer;
            _metadataRepository = metadataRepository;
            _mediaService = mediaService;
        }

        public async Task<string> Gallery(string shortCode)
        {
            //var code = Regex.Replace(shortCode, @"\[|\]", string.Empty);

            var codeItems = shortCode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (codeItems.Contains(WordPressUrSlider) && codeItems.Contains("simple"))
            {
                var id = ExtractId(shortCode);

                var images = await GetGalleryImagesByIdAsync(id);

                if (images.Count == 0) return shortCode;

                return await _renderer.RenderToStringAsync("_SimpleGalleryPartial", new ImageSlider(images, $"slider{id}"));
            }

            if (codeItems.Contains(WordPressUrSlider) && codeItems.Contains("slider"))
            {
                var id = ExtractId(shortCode);

                var images = await GetGalleryImagesByIdAsync(id);

                if (images.Count == 0) return shortCode;

                return await _renderer.RenderToStringAsync("_ImageSliderPartial", new ImageSlider(images, $"slider{id}"));
            }

            if (codeItems.Contains(WordPressUrSlider) && codeItems.Contains("featured"))
            {
                var id = ExtractId(shortCode);

                var images = await GetGalleryImagesByIdAsync(id);

                if (images.Count == 0) return shortCode;

                await AddImagesDescriptionAsync(images);

                return await _renderer.RenderToStringAsync("_FeaturedGalleryPartial", new ImageSlider(images, $"slider{id}"));
            }

            if (codeItems.Contains(WordPressUrSlider))
            {
                var id = ExtractId(shortCode);

                var images = await GetGalleryImagesByIdAsync(id);

                if (images.Count == 0) return shortCode;

                await AddImagesDescriptionAsync(images);

                return await _renderer.RenderToStringAsync("_GalleryPartial", new ImageSlider(images, $"slider{id}"));
            }

            //if (codeItems.Count == 1)
            //{
            //    return await Task.Run(() => "<b>This is processed short code</b>");
            //}

            //if (codeItems.Count == 2 && codeItems.Contains("url"))
            //{
            //    return await Task.Run(() => "<b>This is processed short code with url</b>");
            //}

            //if (codeItems.Count == 3 && codeItems.Contains("url") && codeItems.Contains("img"))
            //{
            //    return await Task.Run(() => "<b>This is processed short code with url and img</b>");
            //}

            // display if you want to remove the brackets or remove completely missing short codes
            return shortCode;
        }

        private async Task AddImagesDescriptionAsync(List<SlideItem> images)
        {
            var titleDescriptionList = await _mediaService.GetDescription(images.Select(s => s.Url).Distinct().ToDictionary(k => k, _ => ("", "")));

            images = images.Select(s =>
            {
                var hasKey = titleDescriptionList.ContainsKey(s.Url);

                var description = string.IsNullOrWhiteSpace(s.Description) && hasKey ? titleDescriptionList[s.Url].description : s.Description;
                var title = string.IsNullOrWhiteSpace(s.Title) && hasKey ? titleDescriptionList[s.Url].title : s.Title;

                s.Description = string.IsNullOrWhiteSpace(description) ? description : description.Replace("\r\r\n", "<br />").Replace("\r\n", "<br />").Replace("\n", "<br />");
                s.Title = title;

                return s;

            }).ToList();
        }

        private static int ExtractId(string shortCode)
        {
            var number = Regex.Replace(shortCode, @"[^\d]", "");

            int.TryParse(number, out int id);

            return id;
        }

        public async Task<List<SlideItem>> GetGalleryImagesByIdAsync(int id)
        {
            var items = new List<SlideItem>();

            if (id <= 0) return items;

            var meta = await _metadataRepository.GetPlainAsync(id);

            var galleryMeta = meta.FirstOrDefault(s => s.Key == GalleryMetaKey);

            if (galleryMeta == null) return items;

            return IsBase64String(galleryMeta.Value)
                ? GetBase64SliderData(galleryMeta.Value.Base64ForUrlDecode())
                : await GetSliderDataAsync(galleryMeta.Value);
        }

        private async Task<List<SlideItem>> GetSliderDataAsync(string data)
        {
            var items = new List<SlideItem>();

            if (string.IsNullOrWhiteSpace(data)) return items;

            var list = (ArrayList)new Serializer().Deserialize(data);

            if (list == null) return items;

            var ids = new List<int>();

            foreach (Hashtable? hash in list)
            {
                if (hash == null) continue;

                int.TryParse(GetValue(hash, "rpgp_image_id"), out var id);

                if (id != 0) ids.Add(id);
            }

            var images = await _mediaService.GetAsync(ids);

            if (images.Count > 0)
                items.AddRange(images.Select(s => new SlideItem(s.Title, s.Content, s.Guid)));

            return items;
        }

        private List<SlideItem> GetBase64SliderData(string data)
        {
            var items = new List<SlideItem>();

            if (string.IsNullOrWhiteSpace(data)) return items;

            var list = (ArrayList)new Serializer().Deserialize(data);

            if (list == null) return items;

            foreach (Hashtable? hash in list)
            {
                if (hash == null) continue;

                items.Add(new SlideItem(GetValue(hash, "rpgp_image_label"), GetValue(hash, "rpgp_image_desc"), GetValue(hash, "rpgp_image_url")
                ));
            }

            return items;
        }

        private string GetValue(Hashtable hash, string key) => hash.ContainsKey(key) ? hash[key]?.ToString() ?? "" : "";

        public bool IsBase64String(string value)
        {
            value = value.Trim();

            return value.Length % 4 == 0 && Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
