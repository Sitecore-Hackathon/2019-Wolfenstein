using HackathonWeb.Feature.Media.Controllers;
using HackathonWeb.Feature.Media.Interfaces;
using HackathonWeb.Feature.Media.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace HackathonWeb.Feature.Media.Pipelines.IoC
{
    /// <summary>
    /// IOC Register class 
    /// </summary>
    public class RegisterMediaServices : IServicesConfigurator
    {
        /// <summary>
        /// Configure IOC method with DI on Service Collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        public void Configure(IServiceCollection serviceCollection)
        {
            // Add image filter component repository to IOC
            serviceCollection.AddTransient<IImageFilterComponentRepository, ImageFilterComponentRepository>();

            // Add image filter component controller to IOC
            serviceCollection.AddTransient<ImageFilterComponentController>();
        }
    }
}