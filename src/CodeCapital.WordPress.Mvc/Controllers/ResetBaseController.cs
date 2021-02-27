using CodeCapital.WordPress.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeCapital.WordPress.Mvc.Controllers
{
    public class ResetBaseController : Controller
    {
        protected readonly CacheService CacheService;
        protected readonly ResetService ResetService;

        public ResetBaseController(CacheService cacheService, ResetService resetService)
        {
            CacheService = cacheService;
            ResetService = resetService;
        }
    }
}