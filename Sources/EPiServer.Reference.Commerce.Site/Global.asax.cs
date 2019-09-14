using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;
using EPiServer.Reference.Commerce.Shared.Attributes;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site
{
    public class Global : EPiServer.Global
    {
        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { action = "Index", id = UrlParameter.Optional });

            try
            {
                //Workaroud / hack to make Siteattention work
                routes.MapRoute("SaPropertyRender/Render", "SaPropertyRender/Render", new
                {
                    controller = "SaPropertyRender",
                    action = "Render"
                });
                routes.MapRoute("SaSettingsSave/Get", "SaSettingsSave/Get", new
                {
                    controller = "SaSettingsSave",
                    action = "Get"
                });
                routes.MapRoute("SaSettingsSave/Post", "SaSettingsSave/Post", new
                {
                    controller = "SaSettingsSave",
                    action = "Post"
                });
            }
            catch
            {
            }

        }

        protected void Application_Start()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedRequiredAttribute), typeof(RequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedRegularExpressionAttribute), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedEmailAttribute), typeof(RegularExpressionAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedStringLengthAttribute), typeof(StringLengthAttributeAdapter));

            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/Assets/Base/jquery/jquery.min.js",
            });

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}