using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy
{
    /// <summary>
    /// Attribute used for site content types to set default attribute values
    /// </summary>
    public class SiteContentType : ContentTypeAttribute
    {
        public SiteContentType()
        {
            GroupName = SiteTabs.Content;
        }
    }
}
