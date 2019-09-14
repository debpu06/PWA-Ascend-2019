using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.ViewModels;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Controllers
{
    public class ContentRecommendationsBlockController : BlockController<ContentRecommendationsBlock>
    {
        private readonly IPageRouteHelper _pageRouteHelper;
        private readonly IRecommendationService _recommendationService;
        private readonly IContentLoader _contentLoader;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IUrlResolver _urlResolver;

        public ContentRecommendationsBlockController(
            IPageRouteHelper pageRouteHelper,
            IRecommendationService recommendationService,
            IUrlResolver urlResolver,
            IContentLoader contentLoader,
            IContentTypeRepository contentTypeRepository)
        {
            _pageRouteHelper = pageRouteHelper;
            _recommendationService = recommendationService;
            _urlResolver = urlResolver;
            _contentLoader = contentLoader;
            _contentTypeRepository = contentTypeRepository;
        }

        protected PageData CurrentPage => _pageRouteHelper.Page;

        public override ActionResult Index(ContentRecommendationsBlock currentBlock)
        {
            var request = new ContentRecommendationRequestModel
            {
                ContentId = CurrentPage.ContentGuid.ToString(),
                SiteId = SiteDefinition.Current.Id.ToString(),
                LanguageId = CurrentPage.Language.Name,
                NumberOfRecommendations = currentBlock.NumberOfRecommendations
            };

            var recommendationResult = _recommendationService.GetRecommendationContent(HttpContext, request);

            var model = new ContentRecommendationsBlockViewModel() { ContentRecommendationItems = new List<ContentRecommendationItem>() };

            // Advance is an external service don't assume its there and/or that it returns any results
            if (recommendationResult != null)
            {
                foreach (var item in recommendationResult)
                {
                    var sitePageData = item.Content as SitePageData;
                    if (sitePageData == null)
                    {
                        continue;
                    }
                    model.ContentRecommendationItems.Add(new ContentRecommendationItem()
                        {
                            Content = sitePageData,
                            ContentUrl = _urlResolver.GetUrl(item.Content.ContentLink),
                            ContentType = _contentTypeRepository.Load(item.Content.ContentTypeID).Name
                        }
                    );
                }
            }

            return PartialView("~/Views/Blocks/ContentRecommendationsBlock.cshtml", model);
        }
    }
}
