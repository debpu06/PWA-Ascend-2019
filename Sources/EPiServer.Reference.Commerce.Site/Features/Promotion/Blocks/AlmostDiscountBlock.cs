using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Blocks
{
    [ContentType(GUID = "9ab7384f-8d6b-49e6-b3f8-42c32b7e46d7",
        DisplayName = "Almost at Discount",
        Description = "Almost at Discount notifies shopper when they have almost fulfilled a discount",
        GroupName = "Commerce")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-14.png")]
    public class AlmostDiscountBlock : BlockData
    {
    }
}