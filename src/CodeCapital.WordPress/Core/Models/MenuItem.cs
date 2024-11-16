namespace CodeCapital.WordPress.Core.Models;

public class MenuItem
{
    public long MenuId { get; set; }
    public string CustomTitle { get; set; } = default!;
    public string PostTitle { get; set; } = default!;
    public string PostName { get; set; } = default!;
    public string PostType { get; set; } = default!;
    public long PostId { get; set; }
    public long ParentPostId { get; set; }
    public long ParentMenuId { get; set; }
    public string CustomUrl { get; set; } = default!;

    public string GetTitle() => string.IsNullOrWhiteSpace(CustomTitle) ? PostTitle : CustomTitle;

    public string GetUrl() => string.IsNullOrWhiteSpace(CustomUrl) ? "/" + PostName : CustomUrl;

    public string GetSubmenuUrl(string url) => string.IsNullOrWhiteSpace(CustomUrl) ? $"{(ParentPostId > 0 ? url : "")}/{PostName}" : CustomUrl;
}