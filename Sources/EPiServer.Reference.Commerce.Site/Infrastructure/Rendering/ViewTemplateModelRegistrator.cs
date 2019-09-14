using EPiBootstrapArea;
using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Web;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.QuickOrder.Blocks;
using EPiServer.Reference.Commerce.Site.Features.Search.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Rendering
{
    [ServiceConfiguration(typeof(IViewTemplateModelRegistrator))]
    public class ViewTemplateModelRegistrator : IViewTemplateModelRegistrator
    {

        public const string BlockFolder = "~/Views/Blocks/";
        public const string PagePartialsFolder = "~/Views/Shared/PagePartials/";

        public void Register(TemplateModelCollection viewTemplateModelRegistrator)
        {
            viewTemplateModelRegistrator.Add(typeof(TeaserBlock), new TemplateModel
            {
                Name = "TeaserBlockWide",
                Tags = new[] { ContentAreaTags.TwoThirdsWidth, ContentAreaTags.FullWidth },
                AvailableWithoutTag = false,
                Path = BlockPath("TeaserBlockWide.cshtml")
            });

            viewTemplateModelRegistrator.Add(typeof(SitePageData), new TemplateModel
            {
                Name = "PagePartial",
                Inherit = true,
                AvailableWithoutTag = true,
                Path = PagePartialPath("Page.cshtml")
            });

            viewTemplateModelRegistrator.Add(typeof(SitePageData), new TemplateModel
            {
                Name = "PagePartialWide",
                Inherit = true,
                Tags = new[] { ContentAreaTags.TwoThirdsWidth, ContentAreaTags.FullWidth },
                AvailableWithoutTag = false,
                Path = PagePartialPath("PageWide.cshtml")
            });

            viewTemplateModelRegistrator.Add(typeof(IContentData), new TemplateModel
            {
                Name = "NoRendererMessage",
                Inherit = true,
                Tags = new[] { Constants.ContentAreaTags.NoRenderer },
                AvailableWithoutTag = false,
                Path = BlockPath("NoRenderer.cshtml")
            });

            viewTemplateModelRegistrator.Add(typeof(PageData), new TemplateModel
            {
                Name = "PartialPage",
                Inherit = true,
                AvailableWithoutTag = true,
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                Path = "~/Views/Shared/_Page.cshtml"
            });

            viewTemplateModelRegistrator.Add(typeof(DestinationPage), new TemplateModel
            {
                Name = "DestinationPartialWide",
                AvailableWithoutTag = false,
                Tags = new[] { "displaymode-full" },
                Path = "~/Views/DestinationPage/DestinationWide.cshtml",
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
            });

            viewTemplateModelRegistrator.Add(typeof(ToolProduct), new TemplateModel
            {
                Name = "PartialPage",
                Inherit = true,
                AvailableWithoutTag = true,
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                Path = "~/Features/MoseySupply/Views/Shared/PagePartials/_ToolProduct.cshtml"
            });

            viewTemplateModelRegistrator.Add(typeof(ToolVariant), new TemplateModel
            {
                Name = "PartialPage",
                Inherit = true,
                AvailableWithoutTag = true,
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                Path = "~/Features/MoseySupply/Views/Shared/PagePartials/_ToolProduct.cshtml"
            });

            viewTemplateModelRegistrator.Add(typeof(PromotionData), new TemplateModel
            {
                Name = "PartialPage",
                Inherit = true,
                AvailableWithoutTag = true,
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                Path = "~/Features/Promotion/Views/Shared/PagePartials/_Promotion.cshtml"
            });

            viewTemplateModelRegistrator.Add(typeof(GenericNode), new TemplateModel
            {
                Name = "PartialPage",
                AvailableWithoutTag = true,
                Inherit = true,
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                Path = "~/Views/GenericNode/GenericNode.cshtml",
            });

            #region mobile template

            viewTemplateModelRegistrator.Add(typeof(QuickOrderBlock), new TemplateModel
            {
                Name = "QuickOrderBlockMobile",
                Tags = new[] { RenderingTags.Mobile },
                AvailableWithoutTag = false,
                Path = "~/Views/QuickOrderBlock/QuickOrderBlockMobile.cshtml"
            });

            viewTemplateModelRegistrator.Add(typeof(SearchPage), new TemplateModel
            {
                Name = "SearchResultsMobile",
                Tags = new[] { RenderingTags.Mobile },
                AvailableWithoutTag = true,
                Path = "~/Views/Search/Mobile.cshtml",
                TemplateTypeCategory = TemplateTypeCategories.Mvc,
            });
            #endregion

        }

        private static string BlockPath(string fileName)
        {
            return string.Format("{0}{1}", BlockFolder, fileName);
        }

        private static string PagePartialPath(string fileName)
        {
            return string.Format("{0}{1}", PagePartialsFolder, fileName);
        }
    }
}