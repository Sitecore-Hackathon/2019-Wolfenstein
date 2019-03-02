using HackathonWeb.Feature.Media.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.XA.Feature.Media.Repositories;
using ImageRepository = HackathonWeb.Feature.Media.Repositories.ImageRepository;

namespace HackathonWeb.Feature.Media.Pipelines.IoC
{
    public class RegisterMediaServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IImageRepository, ImageRepository>();
            serviceCollection.AddTransient<MediaImageController>();
        }
    }
}