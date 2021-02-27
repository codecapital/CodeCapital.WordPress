using System.Collections.Generic;

namespace CodeCapital.WordPress.Mvc.Models
{
    public class ImageSlider
    {
        public List<SlideItem> Images { get; set; }
        public string Id { get; set; }

        public ImageSlider(List<SlideItem> images, string id)
        {
            Images = images;
            Id = id;
        }
    }
}
