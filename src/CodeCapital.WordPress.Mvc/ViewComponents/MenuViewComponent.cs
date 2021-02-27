using CodeCapital.WordPress.Mvc.ViewModels;
using CodeCapital.WordPress.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Mvc.ViewComponents
{
    // You can use this as template in your project, .NET Core 3. requires all parameters in the tag
    // .NET Core 5.0 should honor default parameters
    // You can override Menu.cshtml by using your website at the same location
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public MenuViewComponent(IMenuService menuService) => _menuService = menuService;

        public async Task<IViewComponentResult> InvokeAsync(int menuId, string homePage)
        {
            var viewModel = new MenuViewModel
            {
                MenuItems = await _menuService.GetMenuAsync(menuId),
                Action = GetRoute("action"),
                Controller = GetRoute("controller"),
                Page = GetRoute("page"),
                Url = HttpContext.Request.Path.Value,
                HomePage = string.IsNullOrWhiteSpace(homePage) ? "/" : homePage
            };

            return View("Components/Menu.cshtml", viewModel);

            string GetRoute(string key) => ViewContext.ActionDescriptor.RouteValues.ContainsKey(key)
                ? ViewContext.ActionDescriptor.RouteValues[key]
                : "";
        }
    }
}
