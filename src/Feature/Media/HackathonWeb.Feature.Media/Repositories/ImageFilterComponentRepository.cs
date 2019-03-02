using System.Web;
using System.Web.Mvc;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Models;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using Sitecore.XA.Feature.Media.Repositories;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;

namespace HackathonWeb.Feature.Media.Repositories
{
    public class ImageFilterComponentRepository : ImageRepository, IImageFilterComponentRepository
    {
        public override IRenderingModelBase GetModel()
        {
            var model = new ImageFilterComponentModel();

            FillBaseProperties(model);
            FillImageFilterProperties(model);

            return model;
        }

        private void FillImageFilterProperties(ImageFilterComponentModel model)
        {
            var item = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(model.Rendering.DataSource));

            model.HasImageCaption = int.Parse(item.Fields["HasImageCaption"].Value) == 1;
            model.HasImageDescription = int.Parse(item.Fields["HasImageDescription"].Value) == 1;
            model.Image = new MvcHtmlString(item.Fields["Image"].Value);
            model.Filter = item.Fields["Filter"].Value;
        }
    }
}