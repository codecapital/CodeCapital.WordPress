using CodeCapital.WordPress.Core.Shortcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class ShortCodeService : IShortCodeService
    {
        private const string ShortCodePattern = @"\[.*?].*?";
        public ShortCodeManager ShortCodeManager { get; }

        public ShortCodeService() => ShortCodeManager = new ShortCodeManager();

        public async Task<string> ProcessAsync(string text) => await InjectTextAsync(text);

        private async Task<string> InjectTextAsync(string text)
        {
            var shortCodes = GetAllAvailableShortCodes(text);

            if (!shortCodes.Any()) return text;

            // maybe we could cache these codes or some of them
            await GetSubstitutionsAsync(shortCodes);

            return Regex.Replace(text, ShortCodePattern, match => ReplaceShortCodes(match, shortCodes));
        }

        /// <summary>
        /// Checking short codes in the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAllAvailableShortCodes(string text) =>
        //var matchCollection = Regex.Matches(text, ShortCodePattern).Cast<Match>();

        //matchCollection

        Regex.Matches(text, ShortCodePattern).Cast<Match>()
            .Select(s => ShortCodeManager.ReOrderSequence(s.Value))
            .Distinct()
            .ToDictionary(key => key, value => "");

        private static string ReplaceShortCodes(Match match, IReadOnlyDictionary<string, string> shortCodes)
        {
            shortCodes.TryGetValue(ShortCodeManager.ReOrderSequence(match.Value), out var text);

            return text;
        }

        /// <summary>
        /// Invokes short code functions
        /// </summary>
        /// <param name="shortCodes"></param>
        /// <returns></returns>
        private async Task GetSubstitutionsAsync(Dictionary<string, string> shortCodes)
        {
            // Find exact match
            foreach (var key in shortCodes.Keys.ToList())
            {
                if (ShortCodeManager.Shortcodes.TryGetValue(key, out var shortCodeItem))
                {
                    shortCodes[key] = await shortCodeItem.Function.Invoke(key);
                }
                else
                {
                    shortCodes[key] = key;
                }
            }

            if (!ShortCodeManager.BroadMatchAvailable) return;

            // Check further for broad match
            foreach (var shortCodeItem in ShortCodeManager.BroadMatchShortcodes)
            {
                foreach (var shortCode in shortCodes.Where(w => w.Key == w.Value).ToList())
                {
                    if (shortCodeItem.Code.Split(' ').All(s => shortCode.Key.Contains(s)))
                    {
                        shortCodes[shortCode.Key] = await shortCodeItem.Function.Invoke(shortCode.Key);
                    }
                }
            }
        }

        // Keep temporarily
        [Obsolete("Do not use", true)]
        private async Task<string> GetSubstitutionAsync(Match match)
        {
            var codes = Regex.Replace(match.ToString(), @"\[|\]", string.Empty);
            //var codes = string.Join("", match.ToString().Split(new[] { "[", "]", "<p>", "</p>" }, StringSplitOptions.None));

            var codeList = codes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (codeList.Count == 1 && codeList.Contains("test"))
            {
                return await Task.Run(() => "[This is processed shortcode]");
            }

            //if (codeList.Count == 1)
            //{
            //    var model = GetClient(codeList[0]);

            //    if (model == null) return codes;

            //    return $"<a target =\"_blank\" rel=\"nofollow noopener\" href=\"{model.Url.Default}\">{model.Title}</a>";
            //}

            //if (codeList.Count == 2 && codeList.Contains("url"))
            //{
            //    var client = GetClient(codeList.FirstOrDefault(o => o != "url"));

            //    return client == null ? "#" : client.Url.Default;
            //}

            //if (codeList.Count == 2 && codeList.Contains("banner"))
            //{
            //    var bannerId = codeList.FirstOrDefault(o => o != "banner");

            //    //bannerId = LegacyBannerMapping(bannerId);

            //    if (string.IsNullOrWhiteSpace(bannerId)) return codes;

            //    return $"<div class='load-banner' data-language={_unitOfWork.AppSettings.Language} data-bannerid='{bannerId}'>Banner</div>";
            //}

            //if (codeList.Count == 2 && codeList.Contains("spincastle"))
            //{
            //    if (codeList.Contains("version")) return _unitOfWork.AppSettings.Version;

            //    if (codeList.Contains("client-license-list")) return GetClientLicenseList();

            //    if (codeList.Contains("clients-table")) return GetClientsTable();

            //    if (codeList.Contains("responsible-gaming-list")) return GetResponsibleGamingList();

            //    if (codeList.Contains("all-games-count")) return await GetAllGamesCountAsync();

            //    if (codeList.Contains("random-games-1")) return await GetRandomGamesAsync(1);

            //    if (codeList.Contains("random-games-2")) return await GetRandomGamesAsync(2);
            //}

            //if (codeList.Count == 3 && codeList.Contains("img") && codeList.Contains("url"))
            //{
            //    var model = GetClient(codeList.FirstOrDefault(o => o != "url" && o != "img"));

            //    if (model == null) return "";

            //    return
            //        $"<a target='_blank' rel='nofollow noopener' href='{model.Url}'><img alt='{model.Title}' src='{_unitOfWork.AppSettings.Url.Logos + model.Id + ".jpg"}' /></a>";
            //}

            return codes;
        }
    }
}
