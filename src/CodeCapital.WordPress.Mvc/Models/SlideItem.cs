namespace CodeCapital.WordPress.Mvc.Models
{
    public class SlideItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public SlideItem(string title, string description, string url)
        {
            Title = title;
            Description = description;
            Url = url;
        }
    }
}
