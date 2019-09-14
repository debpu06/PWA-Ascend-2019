using EPiServer.Core;
using EPiServer.Framework.Web;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class AlloySearchPageController : PageControllerBase<AlloySearchPage>
    {
        private const int MaxResults = 40;
        private readonly SearchService _searchService;
        private readonly ContentSearchHandler _contentSearchHandler;
        private readonly UrlResolver _urlResolver;
        private readonly TemplateResolver _templateResolver;

        public AlloySearchPageController(
            SearchService searchService, 
            ContentSearchHandler contentSearchHandler, 
            TemplateResolver templateResolver,
            UrlResolver urlResolver)
        {
            _searchService = searchService;
            _contentSearchHandler = contentSearchHandler;
            _templateResolver = templateResolver;
            _urlResolver = urlResolver;
        }

        [ValidateInput(false)]
        public ViewResult Index(AlloySearchPage currentPage, string q)
        {
            var model = new SearchContentModel(currentPage)
                {
                    SearchServiceDisabled = !_searchService.IsActive,
                    SearchedQuery = q
                };

            if(!string.IsNullOrWhiteSpace(q) && _searchService.IsActive)
            {
                var hits = Search(q.Trim(),
                    new[] { SiteDefinition.Current.StartPage, SiteDefinition.Current.GlobalAssetsRoot, SiteDefinition.Current.SiteAssetsRoot }, 
                    ControllerContext.HttpContext, 
                    currentPage.Language.Name).ToList();
                model.Hits = hits;
                model.NumberOfHits = hits.Count();
            }

            return View(model);
        }

        /// <summary>
        /// Performs a search for pages and media and maps each result to the view model class SearchHit.
        /// </summary>
        /// <remarks>
        /// The search functionality is handled by the injected SearchService in order to keep the controller simple.
        /// Uses EPiServer Search. For more advanced search functionality such as keyword highlighting,
        /// facets and search statistics consider using EPiServer Find.
        /// </remarks>
        private IEnumerable<SearchHit> Search(string searchText, IEnumerable<ContentReference> searchRoots, HttpContextBase context, string languageBranch)
        {
            var searchResults = _searchService.Search(searchText, searchRoots, context, languageBranch, MaxResults);

            return searchResults.IndexResponseItems.SelectMany(CreateHitModel);
        }

        private IEnumerable<SearchHit> CreateHitModel(IndexResponseItem responseItem)
        {
            var content = _contentSearchHandler.GetContent<IContent>(responseItem);
            if (content != null && HasTemplate(content) && IsPublished(content as IVersionable))
            {
                yield return CreatePageHit(content);
            }
        }

        private bool HasTemplate(IContent content)
        {
            return _templateResolver.HasTemplate(content, TemplateTypeCategories.Page);
        }

        private bool IsPublished(IVersionable content)
        {
            if (content == null)
                return true;
            return content.Status.HasFlag(VersionStatus.Published);
        }

        private SearchHit CreatePageHit(IContent content)
        {
            return new SearchHit
                {
                    Title = content.Name,
                    Url = _urlResolver.GetUrl(content.ContentLink),
                    Excerpt = content is SitePageData ? ((SitePageData) content).TeaserText : string.Empty
                };
        }
    }
}
