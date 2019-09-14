using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    /// Describing which EPiServer PropertyData is mapped with a specific Content's property model
    /// </summary>
    public interface IPropertyModel<TType> : IPropertyModel where TType : PropertyData
    {
        /// <summary>
        /// Type of <see cref="PropertyData"/> (E.g PropertyContentReference or PropertyDoubleList) to be mapped with Content's property view model
        /// </summary>
        TType PropertyDataProperty { get; set; }
    }

    /// <summary>
    /// Define basic information for a Content's property model that is mapped with a specific EPiServer PropertyData
    /// </summary>
    public interface IPropertyModel
    {
        /// <summary>
        /// The name of the Content's Property Model, by default, it's value is the name of the Episerver Property type.(Use property `name` of <see cref="PropertyData"/> in EpiServer Core.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The type name of the EPiServer PropertyData.
        /// </summary>
        string PropertyDataType { get; }
    }
}
