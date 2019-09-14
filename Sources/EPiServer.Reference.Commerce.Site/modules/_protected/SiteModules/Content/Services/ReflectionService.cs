using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// Service handle object creation 
    /// </summary>
    [ServiceConfiguration(typeof(ReflectionService))]
    public class ReflectionService
    {
        private readonly ContentTypeModelRepository _contentTypeModelRepository;

        /// <summary>
        /// Initialize an instance of relection service
        /// </summary>
        /// <param name="contentTypeModelRepository"></param>
        public ReflectionService(ContentTypeModelRepository contentTypeModelRepository)
        {
            _contentTypeModelRepository = contentTypeModelRepository;
        }

        /// <summary>
        /// Create a instance of given type with parameters
        /// </summary>        
        public virtual object CreateInstance(Type type, params object[] args)
        {
            return (type == null) ? null : Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// Get attributes of an Episerver property from content type
        /// </summary>
        public virtual IEnumerable<Attribute> GetAttributes(ContentType contentType, PropertyData prop)
        {
            var propertyDefinition = contentType.PropertyDefinitions.SingleOrDefault(pd => string.Equals(pd.Name, prop.Name, StringComparison.Ordinal));

            if (propertyDefinition == null)
            {
                return GetAttributesFromType(contentType.ModelType, prop.Name);
            }

            var propertyDefinitionModel = _contentTypeModelRepository.GetPropertyModel(contentType.ID, propertyDefinition);

            if (propertyDefinitionModel == null)
            {
                return GetAttributesFromType(contentType.ModelType, prop.Name);
            }

            return propertyDefinitionModel.Attributes.GetAllAttributes<Attribute>();
        }

        /// <summary>
        /// Get Attributes from model type
        /// </summary>
        protected virtual IEnumerable<Attribute> GetAttributesFromType(Type modelType, string propertyWithAttributes)
        {
            if (modelType == null)
            {
                return null;
            }

            var propertyInfo = modelType.GetProperty(propertyWithAttributes);
            if (propertyInfo == null)
            {
                propertyInfo = GetPropertyInfoFromInterface(modelType, propertyWithAttributes);
            }

            if (propertyInfo != null)
            {
                return propertyInfo.GetCustomAttributes(true).Cast<Attribute>();
            }

            return null;
        }

        /// <summary>
        /// Get property info from interface
        /// </summary>
        protected virtual PropertyInfo GetPropertyInfoFromInterface(Type modelType, string propertyWithAttributesName)
        {
            var interfaceAndPropertyName = propertyWithAttributesName.Split('_');

            if (interfaceAndPropertyName.Length == 2)
            {
                var interfaceType = modelType.GetInterface(interfaceAndPropertyName[0], true);

                if (interfaceType != null)
                {
                    return modelType.GetProperty(interfaceAndPropertyName[1], BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                }
            }

            return null;
        }
    }
}