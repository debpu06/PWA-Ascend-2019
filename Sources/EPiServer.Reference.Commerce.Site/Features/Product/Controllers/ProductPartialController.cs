using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Web.Mvc;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class ProductPartialController : PartialContentController<EntryContentBase>
    {
        private readonly IProductService _productService;
        private readonly IContentLoader _contentLoader;
        private readonly IReviewService _reviewService;
        private readonly IRelationRepository _relationRepository;

        public ProductPartialController(IProductService productService, IContentLoader contentLoader, IReviewService reviewService, IRelationRepository relationRepository)
        {
            _productService = productService;
            _contentLoader = contentLoader;
            _reviewService = reviewService;
            _relationRepository = relationRepository;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public override ActionResult Index(EntryContentBase currentContent)
        {
            var productTileViewModel = _productService.GetProductTileViewModel(currentContent);
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            if (startPage?.ShowProductRatingsOnListings ?? default(bool))
            {
                var code = currentContent.Code;
                if (currentContent is VariationContent)
                {
                    var product = (currentContent as VariationContent).GetParentProducts().FirstOrDefault();
                    code = _contentLoader.Get<EntryContentBase>(product)?.Code;
                }
                productTileViewModel.ReviewStatistics = _reviewService.GetRatings(new[] { code }).FirstOrDefault();
            }
                return PartialView("_ProductPartial", productTileViewModel);
        }
    }
}