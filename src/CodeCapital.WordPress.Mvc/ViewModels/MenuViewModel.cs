using CodeCapital.WordPress.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeCapital.WordPress.Mvc.ViewModels
{
    // if necessary move to WordPress 
    public class MenuViewModel
    {
        public string? Url { get; set; }

        public string Controller { get; set; } = default!;

        public string Action { get; set; } = default!;

        public string? Page { get; set; }

        public string? HomePage { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public string AddTarget(MenuItem menu)
            => menu.GetUrl().StartsWith("http") ? "target='_blank' rel='noopener'" : "";

        public bool IsDropDown(long menuId) => MenuItems.Any(s => s.ParentMenuId == menuId);

        public bool IsActive(MenuItem menu) =>
            menu.PostType == "page" && Page?.ToLower() == "/" + menu.PostName
            ||
            menu.PostType == "page" && Url?.ToLower() == "/" + menu.PostName
            ||
            HomePage == menu.PostName && Url == "/"
        //PostName == "home" && controller?.ToLower() == "home" && action?.ToLower() == "index"
        ;

    }
}
