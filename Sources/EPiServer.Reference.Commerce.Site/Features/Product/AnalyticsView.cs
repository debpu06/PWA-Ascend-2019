using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Shell.UIDescriptors;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Features.Product
{
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class AnalyticsView : ViewConfiguration<CatalogContentBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsView"/> class.
        /// </summary>
        public AnalyticsView()
        {
            Key = "analyticsview";
            //LanguagePath = "/product/contentediting/views";
            ControllerType = "epi-cms/widget/IFrameController";
            ViewType = "/ProductAnalytics/";
            IconClass = "epi-iconGraph";
            SortOrder = 900;
            Name = "Analytics";
            Description = "Analytics Description";
            Category = CommerceViewConfiguration.CatalogCategory;
        }
    }
}