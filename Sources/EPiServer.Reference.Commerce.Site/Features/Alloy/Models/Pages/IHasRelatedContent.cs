using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
