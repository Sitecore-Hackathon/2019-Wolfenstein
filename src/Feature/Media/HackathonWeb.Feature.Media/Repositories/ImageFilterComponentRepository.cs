using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Models;
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
            model.HasImageCaption = true; //bool.Parse(Sitecore.Context.Item.Fields["HasImageCaption"].Value);
            model.HasImageDescription = true; //bool.Parse(Sitecore.Context.Item.Fields["HasImageDescription"].Value);
            model.LinkItem = GetLinkItem();
            model.ExternalImageLink = GetLinkItem()?.Url; //Sitecore.Context.Item.Fields["ExternalImageLink"].Value;
            model.Filter = "Median"; //Sitecore.Context.Item.Fields["Filter"].Value;
        }
    }
}