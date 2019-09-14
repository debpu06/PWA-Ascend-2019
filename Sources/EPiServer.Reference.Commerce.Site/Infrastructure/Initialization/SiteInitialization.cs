using EPiServer.Cms.TinyMce.Core;
using EPiServer.Commerce.Marketing.Internal;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Routing;
using EPiServer.Core.Internal;
using EPiServer.Editor;
using EPiServer.Find.ClientConventions;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Find.Commerce;
using EPiServer.Find.Framework;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.B2B.CustomerProcessors;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.B2B.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Destinations;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Features.Facets;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Warehouses.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Indexing;
using EPiServer.Reference.Commerce.Site.Infrastructure.Rendering;
using EPiServer.Reference.Commerce.Site.Infrastructure.WebApi;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CookieService = EPiServer.Reference.Commerce.Site.Features.Market.Services.CookieService;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    [ModuleDependency(typeof(Personalization.Commerce.InitializationModule))]
    public class SiteInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ViewEngines.Engines.Insert(0, new SiteViewEngine());

            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            //Setup Unified Search Conventions

            SearchClient.Instance.Conventions.UnifiedSearchRegistry
            .ForInstanceOf<DestinationPage>()
            .ProjectImageUriFrom(
                page => new Uri(urlResolver.GetUrl(page.Image), UriKind.Relative));

            SearchClient.Instance.Conventions.ForInstancesOf<DestinationPage>().IncludeField(dp => dp.TagString());
            ContentIndexer.Instance.Conventions.ForInstancesOf<GenericImage>().ShouldIndex(x => false);
            ContentIndexer.Instance.Conventions.ForInstancesOf<ImageMediaData>().ShouldIndex(x => false);

            CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes, false);

            

            AreaRegistration.RegisterAllAreas();

            DisablePromotionTypes(context);

            SetupExcludedPromotionEntries(context);

            //This method creates and activates the default Recommendations widgets.
            //It only needs to run once, not every initialization, and only if you use the Recommendations feature.
            //Instructions:
            //* Enter the configuration values for Recommendations in web.config
            //* Make sure that the episerver:RecommendationsSilentMode flag is not set to true.
            //* Uncomment the following line, compile, start site, comment the line again, compile.

            //SetupRecommendationsWidgets(context);

            EPiServer.Global.RoutesRegistered += Global_RoutesRegistered;
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var services = context.Services;

            services.AddSingleton<EPiServer.Tracking.Commerce.IRecommendationContext, RecommendationContext>();
            services.AddSingleton<IFulfillmentWarehouseProcessor, LocationFulfillmentWarehouseProcessor>();

            services.AddSingleton<ICurrentMarket, B2BCurrentMarket>();
            context.StructureMap().Configure(x => x.For<ThumbnailManager>().Use<ExtendedThumbnailManager>());
            //Register for auto injection of edit mode check, should be default life cycle (per request to service locator)
            services.AddTransient<IsInEditModeAccessor>(locator => () => PageEditing.PageIsInEditMode);

            services.Intercept<IUpdateCurrentLanguage>(
                (locator, defaultImplementation) =>
                    new LanguageService(
                        locator.GetInstance<ICurrentMarket>(),
                        locator.GetInstance<CookieService>(),
                        defaultImplementation));

            services.AddTransient<IModelBinderProvider, ModelBinderProvider>();
            services.AddTransient<IBudgetService, BudgetService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IPlacedPriceProcessor, B2BPlacedPriceProcessor>();
            services.AddHttpContextOrThreadScoped<SiteContext, CustomCurrencySiteContext>();
            services.AddTransient(locator => HttpContext.Current.ContextBaseOrNull());
            context.StructureMap().Configure(c => c.For<PromotionEngineContentLoader>().Singleton().Use<CustomPromotionEngineContentLoader>());
            services.AddSingleton<CatalogContentClientConventions, SiteCatalogContentClientConventions>();
            services.AddSingleton<CatalogContentEventListener, SiteCatalogContentEventListener>();
            services.AddSingleton<IFacetRegistry>(new FacetRegistry(GetFacets(context.StructureMap().GetInstance<ICurrentMarket>())));
            services.AddHttpContextScoped<ISearchService, FindSearchService>();
            services.AddSingleton<ServiceAccessor<IContentRouteHelper>>(locator => locator.GetInstance<IContentRouteHelper>);
            
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.StructureMap()));
            GlobalConfiguration.Configure(config =>
            {
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
                config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
                config.Formatters.XmlFormatter.UseXmlSerializer = true;
                config.DependencyResolver = new StructureMapResolver(context.StructureMap());
                config.MapHttpAttributeRoutes();
                config.EnableCors();
            });

            services.Configure<TinyMceConfiguration>(config =>
            {
                config.Default()
                    .AddPlugin("media wordcount anchor code textcolor colorpicker")
                    .Toolbar("formatselect | epi-personalized-content epi-link anchor numlist bullist indent outdent bold italic underline alignleft aligncenter alignright | image epi-image-editor media code | epi-dnd-processor | removeformat | fullscreen | forecolor backcolor | icons")
                    .AddSetting("image_caption", true)
                    .AddSetting("image_advtab", true);

                config.Default()
                    .AddExternalPlugin("icons", "/Assets/Base/TinyMceFontAwesome/fontawesomeicons.js")
                    .AddSetting("extended_valid_elements", "i[class], span")
                    .ContentCss(new[] { "/Assets/Base/TinyMceFontAwesome/fontawesome.min.css" });
            });

            // To support an extended range of characters (outside of what has been defined in RFC1738) in URLs, define UNICODE_CHARACTERS_IN_URL symbol - this should be done both in Commerce Manager and Front-end site.
            // More information about this feature: http://world.episerver.com/documentation/developer-guides/CMS/routing/internationalized-resource-identifiers-iris/

            // The code block below will allow general Unicode letter characters. 
            // To support more Unicode blocks, update the regular expression for ValidUrlCharacters.
            // For example, to support Thai Unicode block, add \p{IsThai} to it.
            // The supported Unicode blocks can be found here: https://msdn.microsoft.com/en-us/library/20bw873z(v=vs.110).aspx#Anchor_12

#if UNICODE_CHARACTERS_IN_URL
            context.Services.RemoveAll<UrlSegmentOptions>();
            context.Services.AddSingleton<UrlSegmentOptions>(s => new UrlSegmentOptions
            {
                Encode = true,
                ValidUrlCharacters = @"\p{L}0-9\-_~\.\$"
            });
#endif
        }

        public void Uninitialize(InitializationEngine context) { }

        private List<FacetDefinition> GetFacets(ICurrentMarket currentMarket)
        {
            var brand = new FacetStringDefinition
            {
                FieldName = "Brand",
                DisplayName = "Brand"
            };

            var color = new FacetStringListDefinition
            {
                DisplayName = "Color",
                FieldName = "AvailableColors"
            };

            var size = new FacetStringListDefinition
            {
                DisplayName = "Size",
                FieldName = "AvailableSizes"
            };

            var priceRanges = new FacetNumericRangeDefinition(currentMarket)
            {
                DisplayName = "Price ",
                FieldName = "DefaultPrice",
                BackingType = typeof(double)

            };
            priceRanges.Range.Add(new SelectableNumericRange() { To = 5 });
            priceRanges.Range.Add(new SelectableNumericRange() { From = 5, To = 10 });
            priceRanges.Range.Add(new SelectableNumericRange() { From = 10, To = 20 });
            priceRanges.Range.Add(new SelectableNumericRange() { From = 20 });

            return new List<FacetDefinition> { priceRanges, brand, size, color };
        }

        void Global_RoutesRegistered(object sender, RouteRegistrationEventArgs e)
        {
            RouteTable.Routes.RegisterPartialRouter(new TagsPartialRouting());
            RouteTable.Routes.RegisterPartialRouter(new DestinationsPartialRouting());
            
        }

        private void DisablePromotionTypes(InitializationEngine context)
        {
            //var promotionTypeHandler = context.Locate.Advanced.GetInstance<PromotionTypeHandler>();

            // To disable one of built-in promotion types, for example the BuyQuantityGetFreeItems promotion, comment out the following codes:
            //promotionTypeHandler.DisablePromotions(new[] { typeof(BuyQuantityGetFreeItems) });

            // To disable all built-in promotion types, comment out the following codes:
            //promotionTypeHandler.DisableBuiltinPromotions();
        }

        private void SetupExcludedPromotionEntries(InitializationEngine context)
        {
            //To exclude some entries from promotion engine we need an implementation of IEntryFilter.
            //In most cases you can just use EntryFilterSettings to configure the default implementation. Otherwise you can create your own implementation of IEntryFilter if needed.

            //var filterSettings = context.Locate.Advanced.GetInstance<EntryFilterSettings>();
            //filterSettings.ClearFilters();

            //Add filter predicates for a whole content type.
            //filterSettings.AddFilter<TypeThatShouldNeverBeIncluded>(x => false);

            //Add filter predicates based on any property of the content type, including implemented interfaces.
            //filterSettings.AddFilter<IInterfaceThatCanBeImplementedToDetermineExclusion>(x => !x.ShouldBeExcluded);

            //Add filter predicates based on meta fields that are not part of the content type models, e.g. if the field is dynamically added to entries in an import or integration.
            //filterSettings.AddFilter<EntryContentBase>(x => !(bool)(x["ShouldBeExcludedPromotionMetaField"] ?? false));

            //Add filter predicates base on codes like below.
            //var ExcludingCodes = new string[] { "SKU-36127195", "SKU-39850363", "SKU-39101253" };
            //filterSettings.AddFilter<EntryContentBase>(x => !ExcludingCodes.Contains(x.Code));
        }
    }
}