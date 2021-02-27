using CodeCapital.WordPress.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeCapital.WordPress.Core.Extensions
{
    /// <summary>
    /// This will extract images names original and thumbnail|medium|medium_large|large|post-thumbnail from a php serializes string
    /// e.g. s:9:"thumbnail";a:5:{s:4:"file";s:21:"beach1140-150x150.jpg"}
    /// </summary>
    public class ImageNameExtractor
    {
        private const string ImageSizes = "thumbnail|medium|medium_large|large|post-thumbnail";

        public static List<Image> Extract(string? serialisedInput)
        {
            var imageList = new List<Image>();

            if (string.IsNullOrWhiteSpace(serialisedInput)) return imageList;

            var imageNameList = ParseString(serialisedInput);

            if (imageNameList.Count == 0) return imageList;

            var originalImage = imageNameList[0];

            imageList.Add(new Image(originalImage, ImageType.Original));

            if (originalImage.Count(c => c == '/') == 0) return imageList;

            var folder = originalImage.Substring(0, originalImage.LastIndexOf('/') + 1);

            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                var key = type.GetDescription();
                var index = imageNameList.IndexOf(key);

                if (index > 0 && index + 1 < imageNameList.Count)
                {
                    imageList.Add(new Image(folder + imageNameList[index + 1], type));
                }
            }

            return imageList;
        }

        private static List<string> ParseString(string serialisedInput)
        {
            var pattern = $@"""(({ImageSizes})|(([^""]*\.(jpe?g|gif|png))))""";

            var imageNameList = Regex.Matches(serialisedInput, pattern);

            return imageNameList.Cast<Match>().Select(s => s.Value.Replace(@"""", "")).ToList();
        }
    }
}
