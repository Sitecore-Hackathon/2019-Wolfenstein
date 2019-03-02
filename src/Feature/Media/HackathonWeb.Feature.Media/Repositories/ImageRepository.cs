using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HackathonWeb.Feature.Media.Models;
using Sitecore.XA.Feature.Media.Repositories;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;

namespace HackathonWeb.Feature.Media.Repositories
{
    public class ImageRepository : ModelRepository, IImageRepository
    {
        public override IRenderingModelBase GetModel()
        {
            ImageRenderingModel imageRenderingModel = new ImageRenderingModel();
            FillBaseProperties(imageRenderingModel);
            imageRenderingModel.HasImageCaption = true; //HasImageCaption();
            imageRenderingModel.HasImageDescription = true; // HasImageDescription();
            imageRenderingModel.LinkItem = null; //GetLinkItem();
            imageRenderingModel.ExternalImageLink = null; //GetExternalImageLink();
            return imageRenderingModel;
        }
    }
}