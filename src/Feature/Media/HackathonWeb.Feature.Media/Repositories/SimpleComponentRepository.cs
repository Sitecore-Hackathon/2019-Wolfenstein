using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Models;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;

namespace HackathonWeb.Feature.Media.Repositories
{
    public class SimpleComponentRepository : ModelRepository, ISimpleComponentRepository
    {
        public override IRenderingModelBase GetModel()
        {
            var model = new SimpleComponentModel();

            FillBaseProperties(model);
            model.Title = GetTitle();

            return model;
        }

        private string GetTitle()
        {
            return "WORKING";
        }
    }
}