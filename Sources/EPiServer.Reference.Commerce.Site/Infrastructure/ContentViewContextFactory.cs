using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using static EPiServer.Reference.Commerce.Site.Infrastructure.Constants;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public class ContentViewContextFactory
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly IDatabaseMode _databaseMode;

        public ContentViewContextFactory(IContentLoader contentLoader, UrlResolver urlResolver, IDatabaseMode databaseMode)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _databaseMode = databaseMode;
        }

        public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, RequestContext requestContext)
        {
            var startPageContentLink = SiteDefinition.Current.StartPage;

            // Use the content link with version information when editing the startpage,
            // otherwise the published version will be used when rendering the props below.
            if (currentContentLink!= null && currentContentLink.CompareToIgnoreWorkID(startPageContentLink))
            {
                startPageContentLink = currentContentLink;
            }

            var startPage = _contentLoader.Get<BaseStartPage>(startPageContentLink);
            var layoutModel = new LayoutModel();

            if (startPage is AlloyStartPage)
            {
                var alloyStartPage = startPage as AlloyStartPage;
                layoutModel.Logotype = alloyStartPage.SiteLogotype;
                layoutModel.LogotypeLinkUrl = new MvcHtmlString(_urlResolver.GetUrl(SiteDefinition.Current.StartPage));
                layoutModel.ProductPages = alloyStartPage.ProductPageLinks;
                layoutModel.CompanyInformationPages = alloyStartPage.CompanyInformationPageLinks;
                layoutModel.NewsPages = alloyStartPage.NewsPageLinks;
                layoutModel.CustomerZonePages = alloyStartPage.CustomerZonePageLinks;
                layoutModel.LoggedIn = requestContext.HttpContext.User.Identity.IsAuthenticated;
                layoutModel.LoginUrl = new MvcHtmlString(GetLoginUrl(currentContentLink));
                layoutModel.SearchPageRouteValues = requestContext.GetPageRoute(alloyStartPage.SearchPageLink);
                layoutModel.SearchActionUrl = new MvcHtmlString(UrlResolver.Current.GetUrl(alloyStartPage.SearchPageLink));
                layoutModel.IsInReadonlyMode = _databaseMode.DatabaseMode == DatabaseMode.ReadOnly;
                layoutModel.MasterName = MasterViewSite.AlloySite;
            }
            else if (startPage is MoseySupplyStartPage)
            {
                layoutModel.MasterName = MasterViewSite.MoseySupplySite;
            }
            else if (startPage is StartPage)
            {
                layoutModel.MasterName = MasterViewSite.MoseySite;
            }

            return layoutModel;
        }

        private string GetLoginUrl(ContentReference returnToContentLink)
        {
            return string.Format(
                "{0}?ReturnUrl={1}",
                (FormsAuthentication.IsEnabled ? FormsAuthentication.LoginUrl : VirtualPathUtility.ToAbsolute(Constants.AppRelativeLoginPath)),
                _urlResolver.GetUrl(returnToContentLink));
        }

        public virtual IContent GetSection(ContentReference contentLink)
        {
            var currentContent = _contentLoader.Get<IContent>(contentLink);
            if (currentContent.ParentLink != null && currentContent.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
            {
                return currentContent;
            }

            return _contentLoader.GetAncestors(contentLink)
                .OfType<PageData>()
                .SkipWhile(x => x.ParentLink == null || !x.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// Indicate a kind of content should not change layout
    /// </summary>
    public interface IUseDirectLayout { }
}
