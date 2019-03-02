using System;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Models;
using HackathonWeb.Feature.Media.Pipelines.ML;
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
            if (string.IsNullOrEmpty(model?.Rendering?.DataSource))
            {
                return;
            }

            var item = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(model.Rendering.DataSource));
            model.HasImageCaption = int.Parse(item.Fields["HasImageCaption"].Value) == 1;
            model.HasImageDescription = int.Parse(item.Fields["HasImageDescription"].Value) == 1;

            var imageValue = item.Fields["Image"].Value;
            model.Id = new Sitecore.Data.ID(imageValue.Substring(imageValue.IndexOf("{", StringComparison.InvariantCultureIgnoreCase), imageValue.IndexOf("}", StringComparison.InvariantCultureIgnoreCase) - imageValue.IndexOf("{", StringComparison.InvariantCultureIgnoreCase) + 1));
            var itemImage = Sitecore.Context.Database.GetItem(model.Id);
            var mediaItem = new Sitecore.Data.Items.MediaItem(itemImage);
            var image = new System.Drawing.Bitmap(mediaItem.GetMediaStream());

            model.OriginalImage = $"data:image/png;base64,{Convert.ToBase64String(ImagePreprocessing.ConvertImageToArray(image))}";
            model.Filter = item.Fields["Filter"].Value;

            var filterParameters = new Pipelines.ML.ImageFilterParameters { Image = image, filter = (Pipelines.ML.EnumImageFilter)Enum.Parse(typeof(Pipelines.ML.EnumImageFilter), model.Filter) };
            var filterImage = ImagePreprocessing.ApplyFilter(filterParameters);
            model.FilterImage = $"data:image/png;base64,{Convert.ToBase64String(ImagePreprocessing.ConvertImageToArray(filterImage))}";
        }
    }
}