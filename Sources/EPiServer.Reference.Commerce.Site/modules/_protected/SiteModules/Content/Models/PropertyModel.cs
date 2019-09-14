using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Framework;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    /// <summary>
    /// Base class implementation for IPropertyModel types
    /// </summary>
    /// <typeparam name="TValue">Type of the Value property. This can be primitive type of reference type.(E.g string, double, bool, CategoryModel...)  </typeparam>
    /// <typeparam name="TType">Type of the Type property inherited from <see cref="PropertyData"/>.
    ///		E.g: TType can be PropertyPageReference or PropertyLongString, etc...
    /// </typeparam>
    public abstract class PropertyModel<TValue, TType> : IPropertyModel<TType> where TType : PropertyData
    {
        /// <summary>
        /// Initialize a new instance of PropertyModel
        /// </summary>
        /// <param name="type">EpiServer's <see cref="PropertyData"/> to map with this property model</param>
        public PropertyModel(TType type)
        {
            Validator.ThrowIfNull(nameof(type), type);
            Name = type.Name;
            PropertyDataProperty = type;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public string Name { get; set; }

        /// <inheritdoc />
        public TValue Value { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public TType PropertyDataProperty { get; set; }

        /// <inheritdoc />
        public string PropertyDataType
        {
            get { return typeof(TType).Name; }
        }

        /// <summary>
        /// Iterate through a list of items and run action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public void ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach (T local in items)
            {
                action(local);
            }
        }
    }
}