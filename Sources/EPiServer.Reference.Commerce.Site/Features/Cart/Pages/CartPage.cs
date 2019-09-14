using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Pages
{
    [ContentType(DisplayName = "Cart Page", GUID = "4d32f8b1-7651-49db-88e2-cdcbec8ed11c", Description = "", GroupName = "Commerce", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/cms-icon-page-08.png")]
    public class CartPage : SitePageData, IProductRecommendations
    {
        [CultureSpecific]
        public virtual ContentArea MainContentArea { get; set; }

        [CultureSpecific]
        [Display(Name = "Show Recommendations", Order = 50, Description = "This will determine whether or not to show recommendations.")]
        public virtual bool ShowRecommendations { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            ShowRecommendations = true;
        }
    }
}