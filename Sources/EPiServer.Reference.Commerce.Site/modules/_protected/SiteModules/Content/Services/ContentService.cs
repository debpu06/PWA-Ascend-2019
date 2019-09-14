using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    [ServiceConfiguration(typeof(IContentService))]
    public class ContentService : IContentService
    {
        private readonly IContentModelMapper _contentModelMapper;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ContentLoaderService _contentLoaderService;
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public ContentService(
            ContentLoaderService contentLoaderService,
            IContentModelMapper contentModelMapper,
            IContentTypeRepository contentTypeRepository,
            IContentRepository contentRepository,
            ILanguageBranchRepository languageBranchRepository)
        {
            _contentLoaderService = contentLoaderService;
            _contentModelMapper = contentModelMapper;
            _contentTypeRepository = contentTypeRepository;
            _contentRepository = contentRepository;
            _languageBranchRepository = languageBranchRepository;
        }

        public ContentModel Get(ContentReference contentReference, string language, string properties = "*")
        {
            var content = _contentLoaderService.Get(contentReference, language);
            return _contentModelMapper.TransformContent(content, false, properties);
        }

        public ContentModel Get(Guid guid, string language, string properties = "*")
        {
            var content = _contentLoaderService.Get(guid, language);
            return _contentModelMapper.TransformContent(content, false, properties);
        }

        public IEnumerable<ContentModel> GetByContentType(int contentTypeId, string language, string properties = "*", string keyword = "")
        {
            var contents = _contentLoaderService.GetByContentType(contentTypeId, language);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                contents = contents.Where(o => o.Name.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0);
            }
            return contents.Select(o => _contentModelMapper.TransformContent(o, false, properties));
        }

        public IEnumerable<object> GetContentTypes(string type)
        {
            return _contentTypeRepository.List().Where(o => o.Name != "SysRoot" && o.Name != "SysRecycleBin" && IsValidType(type, o.ModelType))
                .Select(o => new
                {
                    o.ID,
                    o.GUID,
                    o.Name,
                    o.DisplayName,
                })
                .OrderBy(o => o.Name);
        }

        public IEnumerable<object> GetLanguages()
        {
            return _languageBranchRepository.ListEnabled().Select(o => new
            {
                o.ID,
                o.LanguageID,
                o.Name,
            });
        }

        public IEnumerable<object> GetProperties(int id)
        {
            var contentType = _contentTypeRepository.Load(id);
            var properties = contentType.PropertyDefinitions
                    .Where(o => o.Type.DataType == PropertyDataType.LongString && o.Type.DefinitionType.Name == typeof(PropertyLongString).Name
                        || o.Type.DataType == PropertyDataType.String && o.Type.DefinitionType.Name == typeof(PropertyString).Name
                        || o.Type.DataType == PropertyDataType.Number
                        || o.Type.DataType == PropertyDataType.FloatNumber
                        || o.Type.DataType == PropertyDataType.Boolean
                        || o.Type.DataType == PropertyDataType.Date)
                    .Select(o => new
                    {
                        o.ID,
                        o.Name,
                    });
            return properties;
        }

        public bool UpdateContent(ContentModel contentModel, string properties, out string message)
        {
            try
            {
                var props = properties.Split(',');
                var content = _contentRepository.Get<IContent>(contentModel.ContentGuid);
                if (!(((IReadOnly)content)?.CreateWritableClone() is IContent clone))
                {
                    message = "No IReadonly implementation!";
                    return false;
                }
                foreach (var prop in props)
                {
                    var propData = clone.Property.FirstOrDefault(o => o.Name == prop);
                    propData.Value = contentModel.Properties[prop];
                    clone.Property.Set(prop, propData);
                }
                clone.Name = contentModel.Name;
                _contentRepository.Save(clone, DataAccess.SaveAction.Publish);
                message = "Save Successfully!";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        private bool IsValidType(string type, Type contentType)
        {
            if (string.IsNullOrEmpty(type))
            {
                return false;
            }

            switch (type)
            {
                case "Page":
                    return typeof(PageData).IsAssignableFrom(contentType);
                case "Block":
                    return typeof(BlockData).IsAssignableFrom(contentType);
                case "Media":
                    return typeof(MediaData).IsAssignableFrom(contentType);
                case "Node":
                    return typeof(NodeContent).IsAssignableFrom(contentType);
                case "Entry":
                    return typeof(EntryContentBase).IsAssignableFrom(contentType);
                case "Campaign":
                    return typeof(SalesCampaign).IsAssignableFrom(contentType);
                case "Discount":
                    return typeof(PromotionData).IsAssignableFrom(contentType);
                default:
                    return false;
            }
        }
    }
}