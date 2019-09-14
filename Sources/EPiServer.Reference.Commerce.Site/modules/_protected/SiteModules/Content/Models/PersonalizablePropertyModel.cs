using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    /// <summary>
    /// Mapped property model for <see cref="IPersonalizableProperty"/>
    /// </summary>
    public abstract class PersonalizablePropertyModel<TValue, TType> : PropertyModel<TValue, TType>, IPersonalizableProperty where TType : PropertyData
    {
        public bool ExcludePersonalizedContent { get; set; }

        protected PersonalizablePropertyModel(TType propertyData, bool excludePersonalizedContent) : base(propertyData)
        {
            ExcludePersonalizedContent = excludePersonalizedContent;
        }
    }
}