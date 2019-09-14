using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    /// Interface for mapping content to custom models
    /// </summary>
    public interface IContentModelMapper
    {
        /// <summary>
        /// List property model converters to convert EpiServer's <see cref="PropertyData"/> to Content's property model
        /// </summary>
        IEnumerable<IPropertyModelConverter> PropertyModelConverters { get; }

        /// <summary>
        /// Maps an instance of IContent to ContentModel
        /// </summary>
        /// <param name="content">The IContent object that the ContentModel is generated from</param>
        /// <param name="excludePersonalizedContent">Boolean to indicate whether or not to return personalization data in the instance of the ContentModel</param>
        /// <returns>Instance of ContentModel</returns>
        ContentModel TransformContent(IContent content, bool excludePersonalizedContent = false, string expand = "");
    }
}
