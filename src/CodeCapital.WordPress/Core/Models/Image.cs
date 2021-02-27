namespace CodeCapital.WordPress.Core.Models
{
    public class Image
    {
        public ImageType Type { get; set; }
        public string Name { get; set; }

        public Image(string name, ImageType type)
        {
            Name = name;
            Type = type;
        }
    }
}
