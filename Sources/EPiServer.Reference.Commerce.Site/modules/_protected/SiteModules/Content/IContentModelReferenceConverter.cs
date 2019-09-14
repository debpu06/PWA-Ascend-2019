using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    /// interface for generating instances of ContentModelReference
    /// </summary>
    public interface IContentModelReferenceConverter
    {
        /// <summary>
        /// Returns an instance of ContentModelReference based on provided IContent
        /// </summary>
        /// <param name="content">IContent instance</param>
        /// <returns>ContentModelReference</returns>
        ContentModelReference GetContentModelReference(IContent content);

        /// <summary>
        /// Returns an instance of ContentModelReferece based on provided ContentReference
        /// </summary>
        /// <param name="contentReference">ContentReference instance</param>
        /// <returns>ContentModelReference</returns>
        ContentModelReference GetContentModelReference(ContentReference contentReference);
    }
}
