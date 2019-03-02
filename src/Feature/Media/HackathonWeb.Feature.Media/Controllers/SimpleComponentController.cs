using System.Web.Mvc;
using HackathonWeb.Feature.Media.Interfaces;
using Sitecore.XA.Foundation.Mvc.Controllers;

namespace HackathonWeb.Feature.Media.Controllers
{
    public class SimpleComponentController : StandardController
    {
        private readonly ISimpleComponentRepository _repository;

        public SimpleComponentController(ISimpleComponentRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult SimpleComponentIndex()
        {
            return PartialView("~/Views/Media/SimpleComponent.cshtml", this.GetModel());
        }

        protected override object GetModel()
        {
            return _repository.GetModel();
        }
    }
}