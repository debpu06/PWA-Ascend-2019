using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public abstract class BaseProductController<T> : ContentController<T> where T : EntryContentBase
    {
        private readonly IReviewService _reviewService;
        private readonly IReviewActivityService _reviewActivityService;

        protected BaseProductController(IReviewActivityService reviewActivityService,
            IReviewService reviewService)
        {
            _reviewActivityService = reviewActivityService;
            _reviewService = reviewService;
        }

        protected ReviewsViewModel GetReviews(string productCode)
        {

            //Testing to query FIND with GetRatingAverage
            //var searchClient = Client.CreateFromConfig();
            //var contentResult = searchClient.Search<FashionProduct>()
            //                .Filter(c => c.GetRatingAverage().GreaterThan(0))
            //                .OrderByDescending(c => c.GetRatingAverage()).Take(25)
            //                .GetContentResult();


            // Return reviews for the product with the ReviewService
            return _reviewService.Get(productCode);
        }

        protected void AddActivity(string product,
            int rating,
            string user)
        {
            // Create the review activity
            var activity = new ReviewActivity
            {

                Product = product,
                Rating = rating,
                Contributor = user,
            };

            // Add the review activity 
            _reviewActivityService.Add(user, product, activity);
        }
    }
}