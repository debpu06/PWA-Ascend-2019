using System.Globalization;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    /// <summary>
    /// Allow to define data type returned for expand value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExpandableProperty<T> : IExpandableProperty
    {
        /// <summary>
        /// Store expanded properties
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        T ExpandedValue { get; set; }
    }

    /// <summary>
    /// Normally, a property only contains the basic information (e.g. contentlink : {name, workid} ). 
    /// But for some special ones, clients may want to get full information of it (e.g. contentlink: {name, workid, publishdate, editdate, etc}), 
    /// so in this case, those properties should inherit this interface.
    /// </summary>
    public interface IExpandableProperty : IPropertyModel
    {
        /// <summary>
        /// Retrieve all sub-properties of expanded property with given language and store into ExpandedValue/>
        /// </summary>
        /// <param name="language"></param>
        void Expand(CultureInfo language);
    }
}