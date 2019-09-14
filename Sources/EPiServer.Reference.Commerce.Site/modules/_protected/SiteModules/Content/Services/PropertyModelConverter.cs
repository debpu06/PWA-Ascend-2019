using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// The default implementation of <see cref="IPropertyModelConverter"/>. 
    /// This class is used for handling the mapping between EPiServer property data to property models.
    /// </summary>
    [ServiceConfiguration(typeof(IPropertyModelConverter), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PropertyModelConverter : IPropertyModelConverter
    {
        private readonly ReflectionService _reflectionService;

        /// <summary>
        /// Initialize a new instance of <see cref="DefaultPropertyModelConverter"/>
        /// </summary>
        public PropertyModelConverter() : this(ServiceLocator.Current.GetInstance<ReflectionService>())
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="DefaultPropertyModelConverter"/>
        /// </summary>
        /// <param name="reflectionService"></param>
        public PropertyModelConverter(ReflectionService reflectionService)
        {
            _reflectionService = reflectionService;
            ModelTypes = InitializeModelTypes();
        }

        /// <summary>
        /// Initialize the mapping between Content API's <see cref="IPropertyModel"/> with corresponding EpiServer.Core's <see cref="PropertyData"/>
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TypeModel> InitializeModelTypes()
        {
            var modelTypes = new List<TypeModel>();

            // get all concrete classes of IPropertyModel
            var propertyModels = ReflectionHelper.GetConcreteDerivedTypes(typeof(IPropertyModel));

            foreach (var propModel in propertyModels)
            {
                // get all parent generic types e.g: IPropertyModel<>  PropertyModel<,>  PersonalizablePropertyModel<,> etc..
                var baseGenericTypes = ReflectionHelper.GetParentGenericTypes(propModel);

                // only get base generic type  IPropertyModel<>
                var iPropertyModel = baseGenericTypes?.SingleOrDefault(t => string.Equals(t.Name, typeof(IPropertyModel<>).Name, StringComparison.OrdinalIgnoreCase));
                if (iPropertyModel == null)
                {
                    continue;
                }

                // get generic argument, because IPropertyModel<> has only one argument type which is inherited from EPiServer.Core.PropertyData
                var propertyDataProperty = iPropertyModel.GetGenericArguments()[0];

                modelTypes.Add(new TypeModel
                {
                    ModelType = propModel,
                    ModelTypeString = propModel.FullName,
                    PropertyType = propertyDataProperty
                });
            }

            return modelTypes;
        }

        /// <inheritdoc />
        public virtual int SortOrder { get; } = 0;

        /// <inheritdoc />
        public IEnumerable<TypeModel> ModelTypes { get; set; }

        /// <summary>
        /// Method to determine whether or not the provided PropertyData can be represented by
        ///  one of the implementations of IPropertyModel registered in the PropertyModelConverter
        /// </summary>
        /// <param name="propertyData">Instance of PropertyData to check against the IPropertyModels registered 
        /// with the PropertyModelConverter</param>
        /// <returns>boolean indicating whether or not the PropertyModelConverter can provide IPropertyModel
        /// for provided PropertyData</returns>
        public bool HasPropertyModelAssociatedWith(PropertyData propertyData)
        {
            if (propertyData == null)
            {
                return false;
            }
            return ModelTypes.Any(x => x.PropertyType == propertyData.GetType());
        }

        /// <summary>
        /// Based on the provided PropertyData and the registered PropertyModelConverter, 
        /// an instance of IPropertyModel is generated and returned.
        /// </summary>
        /// <param name="propertyData">Instance of PropertyData which the IPropertyModel result is generated from.</param>
        /// <param name="language">language to get value of property</param>
        /// <param name="excludePersonalizedContent">Boolean to indicate whether or not to serialize personalization data.</param>
        /// <param name="expand">
        /// Booolean to indicate whether or not to expand property. 
        /// Normally, a property only contains the basic information (e.g. contentlink : {name, workid} ). But for some special ones, clients may want to get 
        /// full information of it (e.g. contentlink: {name, workid, publishdate, editdate, etc}).
        /// In this case, we will call ConvertToPropertyModel with expand set to true
        /// </param>
        /// <returns>Instance of IPropertyModel</returns>
        public virtual IPropertyModel ConvertToPropertyModel(PropertyData propertyData, CultureInfo language, bool excludePersonalizedContent, bool expand = false)
        {
            if (propertyData == null)
            {
                return null;
            }

            var modelType = ModelTypes.FirstOrDefault(x => x.PropertyType == propertyData.GetType());
            if (modelType != null)
            {
                IPropertyModel model;
                if (typeof(IPersonalizableProperty).IsAssignableFrom(modelType.ModelType))
                {
                    model = (IPropertyModel)_reflectionService.CreateInstance(modelType.ModelType, propertyData, excludePersonalizedContent);
                }
                else
                {
                    model = (IPropertyModel)_reflectionService.CreateInstance(modelType.ModelType, propertyData);
                }

                //If property need to be expand and is an instance of IExpandableProperty, call Expand() of this property
                if (expand)
                {
                    var expandableModel = model as IExpandableProperty;
                    if (expandableModel != null)
                    {
                        expandableModel.Expand(language);
                    }
                }

                return model;
            }

            return null;
        }
    }
}