using Sitecore.XA.Feature.Media.Models;

namespace HackathonWeb.Feature.Media.Models
{
    public class ImageFilterComponentModel : ImageRenderingModel
    {
        public Sitecore.Data.ID Id { get; set; }

        public bool HasImageCaption { get; set; }

        public bool HasImageDescription { get; set; }

        public string Filter { get; set; }

        public int Angle { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string OriginalImage { get; set; }

        public string FilterImage { get; set; }
    }
}