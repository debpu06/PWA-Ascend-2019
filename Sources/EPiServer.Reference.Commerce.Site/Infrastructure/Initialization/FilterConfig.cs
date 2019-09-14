using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.CampaignMailId.Filters;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Reference.Commerce.Site.Infrastructure.VisitorGroupUIStyling;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    /// <summary>
    /// Module for registering filters which will be applied to controller actions.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FilterConfig : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalFilters.Filters.Add(context.Locate.Advanced.GetInstance<ContentContextActionFilter>());
            GlobalFilters.Filters.Add(context.Locate.Advanced.GetInstance<OverrideVisitorGroupCssAttribute>());
            GlobalFilters.Filters.Add(context.Locate.Advanced.GetInstance<MailIdFilter>());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new ReadOnlyFilter());
            GlobalFilters.Filters.Add(new PageTrackingWrapperAttribute());
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
