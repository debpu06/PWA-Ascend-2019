using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    /// interface for determining which model types handle which propertydata types.
    /// </summary>
    public interface IPropertyModelConverter
    {
        /// <summary>
        /// The implementation converter which has higher order will be used to get the value of <see cref="PropertyData"/>
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Contains mapping list between Content's <see cref="IPropertyModel"/> with corresponding EpiServer.Core's <see cref="PropertyData"/>
        /// </summary>
        IEnumerable<TypeModel> ModelTypes { get; }

        /// <summary>
        /// Verifies that the instance of IPropertyModelConverter has the correct IPropertyModel registered for the provided PropertyData type.
        /// </summary>
        /// <param name="propertyData">instance of PropertyData to check</param>
        /// <returns>bool</returns>
        bool HasPropertyModelAssociatedWith(PropertyData propertyData);

        /// <summary>
        /// Returns an instance of IPropertyModel based from the provided PropertyData
        /// </summary>
        /// <param name="propertyData">Instance of PropertyData which the IPropertyModel result is generated from.</param>
        /// <param name="language">Culture to retrieve the property value (if expanding)</param>
        /// <param name="excludePersonalizedContent">Boolean to indicate whether or not to return personalization data in the property model.</param>
        /// <param name="expand">Indicates whether this property should be expanded if it is a reference property</param>
        /// <returns>Instance of IPropertyModel</returns>
        IPropertyModel ConvertToPropertyModel(PropertyData propertyData, CultureInfo language, bool excludePersonalizedContent, bool expand);
    }
}
