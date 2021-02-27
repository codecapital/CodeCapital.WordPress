using CodeCapital.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.Controllers
{
    public abstract class RssController : Controller
    {
        private readonly RssService _rssService;

        public abstract FeedItem ChannelFeed { get; set; }

        public RssController(RssService rssService) => _rssService = rssService;

        public async Task<IActionResult> Index()
        {
            var items = await GetFeedsAsync();

            return Content(_rssService.GetRssFeed(ChannelFeed, items), "application/xml");
        }

        public abstract Task<List<FeedItem>> GetFeedsAsync();
    }
}