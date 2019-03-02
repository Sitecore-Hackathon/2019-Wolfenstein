using System.Drawing;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    public class ImageFilterParameters
    {
        public EnumImageFilter filter { get; set; }
        public Bitmap Image { get; set; }
        public int? Angle { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
    }
}