using CodeCapital.WordPress.Core;
using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Mvc.ViewModels;
using CodeCapital.WordPress.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.Controllers
{
    public class ResetController : ResetBaseController
    {
        public ResetController(CacheService cacheService, ResetService resetService) : base(cacheService, resetService) { }

        public async Task<IActionResult> Index()
        {
            await ResetService.ResetLatest();

            return View();
        }

        public IActionResult Debugger()
        {
            var viewModel = new DebuggerViewModel
            {
                Terms = CacheService.Get<List<Term>>(Constants.TermCacheKey) ?? new List<Term>(),
                Posts = CacheService.Get<ConcurrentDictionary<string, Post>>(Constants.PostCacheKey) ??
                        new ConcurrentDictionary<string, Post>(),
                Menus = CacheService.Get<ConcurrentDictionary<int, List<MenuItem>>>(Constants.MenuCacheKey) ??
                        new ConcurrentDictionary<int, List<MenuItem>>()
            };

            return View(viewModel);
        }
    }
}
