using System.Drawing;
using System.Web.Mvc;
using HackathonWeb.Feature.Media.Interfaces;
using Sitecore.XA.Foundation.Mvc.Controllers;

namespace HackathonWeb.Feature.Media.Controllers
{
    public class ImageFilterComponentController : StandardController
    {
        private readonly IImageFilterComponentRepository _repository;

        public ImageFilterComponentController(IImageFilterComponentRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult ImageFilterComponentIndex()
        {
            return PartialView("~/Views/Media/ImageFilterComponent.cshtml", this.GetModel());
        }

        protected override object GetModel()
        {
            return _repository.GetModel();
        }

        public ActionResult GetFile(Bitmap image)
        {
            var array = Pipelines.ML.ImagePreprocessing.ConvertImageToArray(image);
            return File(array, "image/jpeg");
        }
    }
}