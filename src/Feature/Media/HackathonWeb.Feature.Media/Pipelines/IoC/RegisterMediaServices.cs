using HackathonWeb.Feature.Media.Controllers;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using SimpleComponentRepository = HackathonWeb.Feature.Media.Repositories.SimpleComponentRepository;

namespace HackathonWeb.Feature.Media.Pipelines.IoC
{
    public class RegisterMediaServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISimpleComponentRepository, SimpleComponentRepository>();
            serviceCollection.AddTransient<SimpleComponentController>();

            serviceCollection.AddTransient<IImageFilterComponentRepository, ImageFilterComponentRepository>();
            serviceCollection.AddTransient<ImageFilterComponentController>();
        }
    }
}