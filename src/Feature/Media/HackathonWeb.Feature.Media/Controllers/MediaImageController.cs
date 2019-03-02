using System.Web.Mvc;
using Sitecore.XA.Feature.Media.Repositories;
using Sitecore.XA.Foundation.Mvc.Controllers;

namespace HackathonWeb.Feature.Media.Controllers
{
    public class MediaImageController : StandardController
    {
        protected readonly IImageRepository ImageRepository;

        public MediaImageController(IImageRepository imageRepository)
        {
            this.ImageRepository = imageRepository;
        }

        public ActionResult ReusableImageIndex()
        {
            return PartialView("/Media/Image", this.GetModel());
        }

        protected override object GetModel()
        {
            return ImageRepository.GetModel();
        }
    }
}