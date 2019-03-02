using System.Drawing;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    /// <summary>
    /// Image filter parameters for processing
    /// </summary>
    public class ImageFilterParameters
    {
        // Filter to be applied to a bitmap
        public EnumImageFilter Filter { get; set; }

        // Bitmap original image
        public Bitmap Image { get; set; }

        // Image angle to be rotated
        public int? Angle { get; set; }

        // Image height
        public int? Height { get; set; }

        // Image width
        public int? Width { get; set; }
    }
}