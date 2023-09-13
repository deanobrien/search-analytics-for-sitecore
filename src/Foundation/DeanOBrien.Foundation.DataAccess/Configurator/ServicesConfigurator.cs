using DeanOBrien.Foundation.DataAccess.SearchAnalytics;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace DeanOBrien.Foundation.DataAccess.Configurator
{
    class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISearchStore, SqlSearchStore>();
        }
    }
}
