using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// Provides functionality for converting from IContent or ContentReference to ContentReferenceModel
    /// </summary>
    [ServiceConfiguration(typeof(IContentModelReferenceConverter), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentModelReferenceConverter : IContentModelReferenceConverter
    {
        private readonly IPermanentLinkMapper _linkMapper;

        public ContentModelReferenceConverter(IPermanentLinkMapper linkMapper)
        {
            _linkMapper = linkMapper;
        }

        /// <summary>
        /// Maps Instance of IContent to ContentModelReference
        /// </summary>
        /// <param name="content">Instance of IContent</param>
        /// <returns>Instance of ContentModelReference based on provided IContent</returns>
        public virtual ContentModelReference GetContentModelReference(IContent content)
        {
            if (content == null)
            {
                return null;
            }

            return new ContentModelReference
            {
                Id = content.ContentLink?.ID,
                GuidValue = content.ContentGuid,
                WorkId = content.ContentLink?.WorkID,
                ProviderName = content.ContentLink?.ProviderName
            };
        }

        /// <summary>
        /// Maps Instance of ContentReference to ContentModelReference
        /// </summary>
        /// <param name="contentReference">Instance of IContent</param>
        /// <returns>Instance of ContentModelReference based on provided ContentReference</returns>
        public virtual ContentModelReference GetContentModelReference(ContentReference contentReference)
        {
            if (contentReference == null)
            {
                return null;
            }

            return new ContentModelReference
            {
                Id = contentReference.ID,
                GuidValue = _linkMapper.Find(contentReference)?.Guid,
                WorkId = contentReference.WorkID,
                ProviderName = contentReference.ProviderName
            };
        }
    }
}