using System.Drawing;
using System.Web.Mvc;
using Sitecore.XA.Feature.Media.Models;

namespace HackathonWeb.Feature.Media.Models
{
    public class ImageFilterComponentModel : ImageRenderingModel
    {
        public Sitecore.Data.ID Id{get;set;}
        public bool HasImageCaption { get; set; }

        public bool HasImageDescription { get; set; }

        

        public string Filter { get; set; }

        public Bitmap OriginalImage { get; set; }

        public Bitmap FilterImage { get; set; }
    }
}