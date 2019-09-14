using EPiServer.Find;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Find.UI;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class FindPageController :  PageControllerBase<FindPage>
    {
        private const int MaxResults = 1000;
        private readonly IClient _searchClient;
        private readonly IFindUIConfiguration _findUIConfiguration;
        private readonly IServiceLocator _serviceLocator;
        private readonly ISearchService _findSearchService;

        public FindPageController(IClient searchClient, IFindUIConfiguration findUIConfiguration, ISearchService findSearchService, IServiceLocator serviceLocator)
            
        {
            _searchClient = searchClient;
            _findUIConfiguration = findUIConfiguration;
            _serviceLocator = serviceLocator;
            _findSearchService = findSearchService;
        }

        [ValidateInput(false)]
        public ViewResult Index(FindPage currentPage, FilterOptionViewModel filterOption)
        {

            filterOption.PageSize = currentPage.PageSize;
            filterOption.HighlightExcerpt = currentPage.HighlightExcerpts;
            filterOption.HighlightTitle = currentPage.HighlightTitles;

            var model = _findSearchService.SearchContent(filterOption);
            model.CurrentContent = currentPage;
            model.PublicProxyPath = _findUIConfiguration.AbsolutePublicProxyPath();
            model.PageUrl = _serviceLocator.GetInstance<UrlResolver>().GetVirtualPath(currentPage).VirtualPath;

            return View(model);
        }
       
    }
}
