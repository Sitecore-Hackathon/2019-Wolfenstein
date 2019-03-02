using System.Web.Mvc;
using HackathonWeb.Feature.Media.Interfaces;
using Sitecore.XA.Foundation.Mvc.Controllers;

namespace HackathonWeb.Feature.Media.Controllers
{
    /// <summary>
    /// SXA Image Filter Component Controller 
    /// </summary>
    public class ImageFilterComponentController : StandardController
    {
        private readonly IImageFilterComponentRepository _repository;

        /// <summary>
        /// Image Filter Component controller constructor with DI on Image Filter Component Repository
        /// </summary>
        /// <param name="repository">Image Filter Repository</param>
        public ImageFilterComponentController(IImageFilterComponentRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Main method to get the model for the image filter component
        /// </summary>
        /// <returns>View model</returns>
        public ActionResult ImageFilterComponentIndex()
        {
            return PartialView("~/Views/Media/ImageFilterComponent.cshtml", this.GetModel());
        }

        /// <summary>
        /// Get model from repository
        /// </summary>
        /// <returns>Model as an object</returns>
        protected override object GetModel()
        {
            var model = _repository.GetModel();
            return model;
        }
    }
}