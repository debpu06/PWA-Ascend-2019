using System;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    /// <summary>
    /// This class defines the relation between data types of EPiServer and that of ContentApi for data handling before returning it to clients.
    /// </summary>
    public class TypeModel
    {
        /// <summary>
        /// type of Episerver property
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// The type of the class that takes care of the rendering. Needs to implement the generic interface <see cref="IPropertyModel"/>.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// name of Content mapped property's type. Normally the same name as the name of the ModelType.
        /// </summary>
        public string ModelTypeString { get; set; }
    }
}