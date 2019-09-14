using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models
{
    /// <summary>
    /// Allows for reading and writing custom properties.
    /// </summary>
    public interface IHaveProperties
    {
        /// <summary>
        /// Gets the list of key value pairs for dealing with custom properties.
        /// </summary>
        List<PropertyItem> Properties { get; set; }
    }
}