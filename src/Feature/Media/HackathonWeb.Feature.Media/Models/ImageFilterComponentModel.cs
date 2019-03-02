using Sitecore.XA.Feature.Media.Models;

namespace HackathonWeb.Feature.Media.Models
{
    /// <summary>
    /// Model for Image Filter Component
    /// </summary>
    public class ImageFilterComponentModel : ImageRenderingModel
    {
        // ID of the Image Filter Component
        public Sitecore.Data.ID Id { get; set; }

        // If the image has caption
        public bool HasImageCaption { get; set; }

        // If the image has a description
        public bool HasImageDescription { get; set; }

        // Filter to use in the image filter component, Values come from the enum EnumImageFilter.cs
        public string Filter { get; set; }

        // Image angle to rotate 
        public int Angle { get; set; }

        // Image height
        public int Height { get; set; }

        // Image width
        public int Width { get; set; }

        // Original image to show the client as base 64 string
        public string OriginalImage { get; set; }

        // Filtered image to show the client as base 64 string
        public string FilterImage { get; set; }
    }
}