using CodeCapital.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.Controllers
{
    public abstract class SiteMapController : Controller
    {
        private readonly SitemapService _sitemap;
        public abstract string WebsiteHttp { get; }
        public abstract string Website { get; }

        public SiteMapController(SitemapService sitemap) => _sitemap = sitemap;

        [Route("sitemap.xml")]
        public async Task<IActionResult> Index() => Content(_sitemap.GetSiteMap(WebsiteHttp, Website, await GetPagesAsync()), "text/xml");

        public abstract Task<List<string>> GetPagesAsync();
    }
}
