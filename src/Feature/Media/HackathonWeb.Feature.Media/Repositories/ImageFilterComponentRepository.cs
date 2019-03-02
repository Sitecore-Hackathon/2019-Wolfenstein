using System;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Models;
using HackathonWeb.Feature.Media.Pipelines.ML;
using Sitecore.XA.Feature.Media.Repositories;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;

namespace HackathonWeb.Feature.Media.Repositories
{
    /// <summary>
    /// Image Filter Component Repository, inherits from Image Repository
    /// </summary>
    public class ImageFilterComponentRepository : ImageRepository, IImageFilterComponentRepository
    {
        /// <summary>
        /// Get Image Filter Component model
        /// </summary>
        /// <returns>Rendering Model Base</returns>
        public override IRenderingModelBase GetModel()
        {
            var model = new ImageFilterComponentModel();

            FillBaseProperties(model);
            FillImageFilterProperties(model);

            return model;
        }

        /// <summary>
        /// Fill Image Filter Properties from Rendering Data source, if is present
        /// </summary>
        /// <param name="model">Image Filter Component model</param>
        private void FillImageFilterProperties(ImageFilterComponentModel model)
        {
            if (string.IsNullOrEmpty(model?.Rendering?.DataSource))
            {
                return;
            }

            var item = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(model.Rendering.DataSource));
            model.HasImageCaption = !string.IsNullOrEmpty(item.Fields["HasImageCaption"]?.Value) && int.Parse(item.Fields["HasImageCaption"].Value) == 1;
            model.HasImageDescription = !string.IsNullOrEmpty(item.Fields["HasImageDescription"]?.Value) && int.Parse(item.Fields["HasImageDescription"].Value) == 1;

            var imageValue = item.Fields["Image"].Value;
            var firstLeft = imageValue.IndexOf("{", StringComparison.InvariantCultureIgnoreCase);
            var firstRight = imageValue.IndexOf("}", StringComparison.InvariantCultureIgnoreCase);
            model.Id = new Sitecore.Data.ID(imageValue.Substring(firstLeft, firstRight - firstLeft + 1));

            var itemImage = Sitecore.Context.Database.GetItem(model.Id);
            var mediaItem = new Sitecore.Data.Items.MediaItem(itemImage);
            var image = new System.Drawing.Bitmap(mediaItem.GetMediaStream());

            // Save original image as base 64 string
            model.OriginalImage = $"data:image/png;base64,{Convert.ToBase64String(ImagePreprocessing.ConvertImageToArray(image))}";
            model.Filter = item.Fields["Filter"].Value;
            model.Angle = string.IsNullOrEmpty(item.Fields["Angle"].Value) ? 0 : int.Parse(item.Fields["Angle"].Value);
            model.Height = string.IsNullOrEmpty(item.Fields["Height"].Value) ? image.Width : int.Parse(item.Fields["Height"].Value);
            model.Width = string.IsNullOrEmpty(item.Fields["Width"].Value) ? image.Height : int.Parse(item.Fields["Width"].Value);

            // Set the image filter parameters and process the image
            var filterParameters = new ImageFilterParameters
            {
                Image = image,
                Filter = (EnumImageFilter)Enum.Parse(typeof(EnumImageFilter), model.Filter),
                Angle = model.Angle,
                Height = model.Height,
                Width = model.Width
            };
            var filterImage = ImagePreprocessing.ApplyFilter(filterParameters);

            // Save filtered image as base 64 string
            model.FilterImage = $"data:image/png;base64,{Convert.ToBase64String(ImagePreprocessing.ConvertImageToArray(filterImage))}";
        }
    }
}