using EPiServer.Commerce.Marketing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.CouponFilter;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Reporting;
using EPiServer.ServiceLocation;
using EPiServer.ServiceLocation.Compatibility;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class RegisterImplementations : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // Coupon filter for discounts using unique coupon codes from Campaign
            context.Services.AddSingleton<ICouponFilter, CampaignCouponFilter>();
            context.Services.AddSingleton<ICouponUsage,CouponUsageImpl>();
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}