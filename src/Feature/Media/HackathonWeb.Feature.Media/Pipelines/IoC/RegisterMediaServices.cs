using HackathonWeb.Feature.Media.Controllers;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace HackathonWeb.Feature.Media.Pipelines.IoC
{
    public class RegisterMediaServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IImageFilterComponentRepository, ImageFilterComponentRepository>();
            serviceCollection.AddTransient<ImageFilterComponentController>();
        }
    }
}