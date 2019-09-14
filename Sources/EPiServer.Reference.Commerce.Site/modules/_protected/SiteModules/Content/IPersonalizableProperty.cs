using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    ///     Interface indicating that a property is Personalized
    /// </summary>
    public interface IPersonalizableProperty
    {
        /// <summary>
        /// Define if Personalized Content should be exclude when retrieve content
        /// </summary>
        [JsonIgnore]
        bool ExcludePersonalizedContent { get; set; }
    }
}