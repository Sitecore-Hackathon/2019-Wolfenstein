using System;
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
            if(model?.Rendering?.DataSource == null)
            {
                return;
            }
                
            var item = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(model.Rendering.DataSource));
            model.HasImageCaption = int.Parse(item.Fields["HasImageCaption"].Value) == 1;
            model.HasImageDescription = int.Parse(item.Fields["HasImageDescription"].Value) == 1;            

            model.Id = new Sitecore.Data.ID(item.Fields["Image"].Value.Replace("<image mediaid=\"", "").Replace("\" />", ""));
            var itemImage = Sitecore.Context.Database.GetItem(model.Id);
            var mediaItem = new Sitecore.Data.Items.MediaItem(itemImage);
            var image = new System.Drawing.Bitmap(mediaItem.GetMediaStream());
            model.OriginalImage = image;
            model.Filter = item.Fields["Filter"].Value;
            var filterParameters = new Pipelines.ML.ImageFilterParameters { Image = image, filter = (Pipelines.ML.EnumImageFilter)Enum.Parse(typeof(Pipelines.ML.EnumImageFilter), model.Filter)};
            model.FilterImage = Pipelines.ML.ImagePreprocessing.ApplyFilter(filterParameters);
        }
    }
}