using System.Web.Mvc;
using Sitecore.XA.Feature.Media.Models;
using Sitecore.XA.Foundation.Multisite.LinkManagers;

namespace HackathonWeb.Feature.Media.Models
{
    public class ImageFilterComponentModel : ImageRenderingModel
    {
        public bool HasImageCaption { get; set; }

        public bool HasImageDescription { get; set; }

        public MvcHtmlString Image { get; set; }

        public LinkItem LinkItem { get; set; }

        public string ExternalImageLink { get; set; }

        public string Filter { get; set; }
    }
}