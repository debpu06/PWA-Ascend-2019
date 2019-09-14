using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// Provides a default implementation of IContentModelMapper to map instance of IContent to ContentModel
    /// </summary>
    [ServiceConfiguration(typeof(IContentModelMapper), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentModelMapper : IContentModelMapper
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentModelReferenceConverter _contentModelService;
        private readonly IUrlResolver _urlResolver;
        private readonly ReflectionService _reflectionService;

        public ContentModelMapper(IContentTypeRepository contentTypeRepository,
            ReflectionService reflectionService,
            IContentModelReferenceConverter contentModelService,
            IUrlResolver urlResolver,
            IEnumerable<IPropertyModelConverter> propertyModelConverters)
        {
            _contentTypeRepository = contentTypeRepository;
            _reflectionService = reflectionService;
            _contentModelService = contentModelService;
            _urlResolver = urlResolver;

            PropertyModelConverters = propertyModelConverters
                .OrderByDescending(x => x.SortOrder)
                .ToList();
        }

        /// <inheritdoc />
        public virtual IEnumerable<IPropertyModelConverter> PropertyModelConverters { get; }

        /// <summary>
        /// Method generates a ContentModel based on the provided IContent.
        /// </summary>
        /// <param name="content">The IContent object that the ContentModel is generated from</param>
        /// <param name="excludePersonalizedContent">Boolean to indicate whether or not to return personalization data in the instance of the ContentModel</param>
        /// <param name="expand"> String contain properties need to be expand, seperated by comma. Eg: expand=MainContentArea,productPageLinks. Pass expand='*' to expand all properties</param>
        /// <returns>Instance of ContentModel</returns>
        public virtual ContentModel TransformContent(IContent content, bool excludePersonalizedContent = false, string expand = "")
        {
            Validator.ThrowIfNull(nameof(content), content);

            var contentModel = new ContentModel
            {
                ContentReference = content.ContentLink,
                ContentLink = _contentModelService.GetContentModelReference(content),
                Name = content.Name,
                ContentGuid = content.ContentGuid,
                ParentLink = _contentModelService.GetContentModelReference(content.ParentLink),
                ContentType = GetAllContentTypes(content).ToList(),
                Language = ExtractLocalLanguage(content),
                MasterLanguage = ExtractMasterLanguage(content),
                ExistingLanguages = ExtractExistingLanguages(content).ToList()
            };


            if (content is IVersionable versionable)
            {
                contentModel.StartPublish = versionable.StartPublish?.ToUniversalTime();
                contentModel.StopPublish = versionable.StopPublish?.ToUniversalTime();
                contentModel.Status = versionable.Status.ToString();
            }

            if (content is IChangeTrackable changeTracking)
            {
                contentModel.Created = changeTracking.Created.ToUniversalTime();
                contentModel.Changed = changeTracking.Changed.ToUniversalTime();
                contentModel.Saved = changeTracking.Saved.ToUniversalTime();
            }

            if (content is IRoutable routeable)
            {
                contentModel.RouteSegment = routeable.RouteSegment;
            }

            var localizableContent = content as ILocalizable;
            contentModel.Url = _urlResolver.GetUrl(content.ContentLink, localizableContent?.Language.Name, new UrlResolverArguments
            {
                ContextMode = Web.ContextMode.Default,
                ForceCanonical = true,
            });

            if (content is ICategorizable categorizable)
            {
                var propertyCategory = new PropertyCategory(categorizable.Category);
                CategoryPropertyModel categoryPropertyModel = new CategoryPropertyModel(propertyCategory);
                contentModel.Properties.Add("Category", categoryPropertyModel);
            }

            if (ExtractPropertyDataCollection(content, expand, excludePersonalizedContent) is Dictionary<string, object> propertyMap && propertyMap.Keys.Any())
            {
                contentModel.Properties = contentModel.Properties.Concat(propertyMap).ToDictionary(x => x.Key, x => x.Value);
            }

            return contentModel;
        }

        private LanguageModel ExtractLocalLanguage(IContent content)
        {
            var localeData = content as ILocale;
            if (localeData == null || localeData.Language == null)
            {
                return null;
            }

            return new LanguageModel()
            {
                DisplayName = localeData.Language.DisplayName,
                Name = localeData.Language.Name
            };
        }

        private LanguageModel ExtractMasterLanguage(IContent content)
        {
            var languageData = content as ILocalizable;
            if (languageData == null || languageData.MasterLanguage == null)
            {
                return null;
            }

            return new LanguageModel()
            {
                DisplayName = languageData.MasterLanguage.DisplayName,
                Name = languageData.MasterLanguage.Name
            };
        }

        private IEnumerable<LanguageModel> ExtractExistingLanguages(IContent content)
        {
            var languageData = content as ILocalizable;
            if (languageData == null || languageData.ExistingLanguages == null)
            {
                return Enumerable.Empty<LanguageModel>();
            }

            return languageData.ExistingLanguages.Select(x => new LanguageModel()
            {
                DisplayName = x.DisplayName,
                Name = x.Name
            });
        }

        private IEnumerable<string> GetAllContentTypes(IContent content)
        {
            var baseTypes = new List<string>();
            if (content is BlockData)
            {
                baseTypes.Add("Block");
            }

            if (content is PageData)
            {
                baseTypes.Add("Page");
            }

            if (content is MediaData)
            {
                baseTypes.Add("Media");
            }

            if (content is VideoData)
            {
                baseTypes.Add("Video");
            }

            if (content is ImageData)
            {
                baseTypes.Add("Image");
            }

            var contentType = GetContentTypeById(content.ContentTypeID);
            baseTypes.Add(contentType.Name);

            return baseTypes;
        }

        private ContentType GetContentTypeById(int contentTypeId)
        {
            var contentType = _contentTypeRepository.Load(contentTypeId);
            if (contentType == null)
            {
                throw new Exception($"Content Type id {contentTypeId} not found.");
            }

            return contentType;
        }

        /// <summary>
        /// Extract property data collection from content
        /// </summary>
        protected virtual IDictionary<string, object> ExtractPropertyDataCollection(IContent content, string expand, bool excludePersonalizedContent)
        {
            var propertyMap = new Dictionary<string, object>();
            var languageData = content as ILocalizable;

            var contentType = GetContentTypeById(content.ContentTypeID);

            //Get list of property need to be expand
            var propertiesToExpand = string.IsNullOrWhiteSpace(expand) ? new List<string>() : expand.Split(',').Select(x => x.ToLowerInvariant());
            var properties = content.Property.Where(x => !x.IsMetaData && (x.Type == PropertyDataType.LongString && x.PropertyValueType.Name == typeof(String).Name
                    || x.Type == PropertyDataType.String && x.PropertyValueType.Name == typeof(String).Name
                    || x.Type == PropertyDataType.Number
                    || x.Type == PropertyDataType.FloatNumber
                    || x.Type == PropertyDataType.Boolean
                    || x.Type == PropertyDataType.Date));
            foreach (var property in properties)
            {
                if (ShouldPropertyBeIgnored(contentType, property))
                {
                    continue;
                }
                if (!propertiesToExpand.Any() || 
                    propertiesToExpand.Any() && propertiesToExpand.Contains(property.Name.ToLowerInvariant()))
                {
                    var propertyModel = property.Type == PropertyDataType.Boolean && property.Value == null ? false : property.Value;
                    propertyMap.Add(property.Name, propertyModel);
                }
            }

            return propertyMap;
        }

        /// <summary>
        /// Decide whether a content type's property should be ignored from data serialization
        /// </summary>
        protected virtual bool ShouldPropertyBeIgnored(ContentType contentType, PropertyData property)
        {
            var propertyAttributes = _reflectionService.GetAttributes(contentType, property);
            return propertyAttributes != null && propertyAttributes.OfType<JsonIgnoreAttribute>().Any();
        }
    }
}