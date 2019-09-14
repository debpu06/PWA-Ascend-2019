using EPiBootstrapArea;
using EPiBootstrapArea.Initialization;
using EPiBootstrapArea.Providers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.Channels;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using StructureMap;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [ModuleDependency(typeof(SetupBootstrapRenderer))]
    public class DisplayInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var mobileChannelDisplayMode = new DefaultDisplayMode("mobile")
            {
                ContextCondition = r => IsMobileDisplayModeActive(r,context)
            };

            DisplayModeProvider.Instance.Modes.Insert(0, mobileChannelDisplayMode);
        }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(ConfigureContainer);
        }

        /// <summary>
        /// Configure the IOC container.
        /// </summary>
        /// <param name="configuration">The IOC container configuration.</param>
        private static void ConfigureContainer(ConfigurationExpression configuration)
        {
            configuration.For<IDisplayModeFallbackProvider>().Use<CustomDisplayModeFallbackDefaultProvider>();
        }

        private static bool IsMobileDisplayModeActive(HttpContextBase httpContext, InitializationEngine context)
        {
            if (httpContext.GetOverriddenBrowser().IsMobileDevice)
            {
                return true;
            }
            var displayChannelService = context.Locate.Advanced.GetInstance<IDisplayChannelService>();
            return displayChannelService.GetActiveChannels(httpContext).Any(x => x.ChannelName == MobileChannel.Name);
        }
    }

    /// <summary>
    /// Custome display mode fallback 
    /// </summary>
    public class CustomDisplayModeFallbackDefaultProvider : DisplayModeFallbackDefaultProvider
    {
        public override List<DisplayModeFallback> GetAll()
        {
            return base.GetAll().Union(new[]
                                       {
                                           new DisplayModeFallback
                                           {
                                               Name = "One sixth width (1/6)",
                                               Tag = "displaymode-one-sixth",
                                               LargeScreenWidth = 2,
                                               MediumScreenWidth = 4,
                                               SmallScreenWidth = 6,
                                               ExtraSmallScreenWidth = 12
                                           }
                                       }).ToList();
        }
    }
}