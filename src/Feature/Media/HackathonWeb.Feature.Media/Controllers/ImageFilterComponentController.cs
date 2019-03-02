using System;
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
            var model = _repository.GetModel();
            return model;
        }

        public ActionResult GetFile(string id)
        {
            byte[] imageBytes = Convert.FromBase64String(id);
            
            return File(imageBytes, "image/jpeg");
        }
    }
}