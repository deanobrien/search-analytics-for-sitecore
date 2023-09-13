using DeanOBrien.Feature.SearchAnalytics.Controllers;
using DeanOBrien.Feature.SearchAnalytics.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace DeanOBrien.Feature.SearchAnalytics.Configurator
{
    class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITrackSearch, TrackSearch>();
            serviceCollection.AddTransient<SearchReportController>();
        }
    }
}
