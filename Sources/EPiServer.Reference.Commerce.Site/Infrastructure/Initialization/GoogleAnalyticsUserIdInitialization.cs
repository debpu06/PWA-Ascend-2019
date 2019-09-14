using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.GoogleAnalytics.Web.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Analytics;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class GoogleAnalyticsUserIdInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(c => c.For<IPluginScript>().Use<GoogleAnalyticsUserIdPluginScript>());
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}