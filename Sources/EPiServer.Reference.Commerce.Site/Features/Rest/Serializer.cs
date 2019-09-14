using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Core.Internal;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Rest
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class Serializer
    {
        private readonly IContentFactory _contentFactory;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentRepository _contentRepository;
        private readonly CategoryRepository _categoryRepository;

        public Serializer(IContentFactory contentFactory,
            IContentTypeRepository contentTypeRepository,
            IContentRepository contentRepository,
            CategoryRepository categoryRepository)
        {
            _contentFactory = contentFactory;
            _contentTypeRepository = contentTypeRepository;
            _contentRepository = contentRepository;
            _categoryRepository = categoryRepository;
            PropertyModelHandlers = ServiceLocator.Current.GetAllInstances<IPropertyModelHandler>()
                .OrderBy(x => x.SortOrder)
                .ToList();
            ModelTypes = PropertyModelHandlers.SelectMany(x => x.ModelTypes).ToList();
        }

        public List<IPropertyModelHandler> PropertyModelHandlers { get; }
        public List<TypeModel> ModelTypes { get; }

        public ContentModel TransformContent(IContent content)
        {
            Validator.ThrowIfNull(nameof(content), content);

            var contentModel = new ContentModel
            {
                Id = content.GetContentModelReference(), 
                Name = content.Name,
                ParentId = content.ParentLink.GetContentModelReference()
            };

            var contentType = _contentTypeRepository.Load(content.ContentTypeID);
            if (contentType == null)
            {
                throw new Exception($"Content Type id {content.ContentTypeID} not found.");
            }

            contentModel.ContentType = contentType.Name;
            var languageData = content as ILocalizable;
            if (languageData != null)
            {
                contentModel.Language = languageData.Language.DisplayName;
            }

            var versionable = content as IVersionable;
            if (versionable != null)
            {
                contentModel.StartPublish = versionable.StartPublish;
                contentModel.StopPublish = versionable.StopPublish;
                contentModel.Status = versionable.Status.ToString();
            }

            var changeTracking = content as IChangeTrackable;
            if (changeTracking != null)
            {
                contentModel.Created = changeTracking.Created;
                contentModel.CreatedBy = changeTracking.CreatedBy;
                contentModel.Modified = changeTracking.Changed;
                contentModel.ModifiedBy = changeTracking.ChangedBy;
            }

            var routeable = content as IRoutable;
            if (routeable != null)
            {
                contentModel.UrlSegment = routeable.RouteSegment;
            }

            var categorizable = content as ICategorizable;
            if (categorizable != null && categorizable.Category != null && categorizable.Category.Any())
            {
                contentModel.Categories = categorizable.Category
                    .Select(x => _categoryRepository.Get(x).Name)
                    .ToList();
            }

            var properties = contentType.PropertyDefinitions;
            foreach (var property in properties)
            {
                if (!content.Property.Contains(property.Name))
                {
                    continue;
                }

                foreach (var handler in PropertyModelHandlers)
                {
                    if (!handler.CanHandleProperty(content.Property[property.Name]))
                    {
                        continue;
                    }
                    contentModel.Properties.Add(handler.GetValue(content.Property[property.Name]));
                    break;
                }
            }
            return contentModel;
        }

        public IContent TransformContentModel(ContentModel contentModel)
        {
            var parentLink = contentModel.ParentId.GetContentReference();
            if (ContentReference.IsNullOrEmpty(parentLink))
            {
                return null;
            }

            IContent content;
            var link = contentModel.Id.GetContentReference();
            if (ContentReference.IsNullOrEmpty(link))
            {
                var contentType = _contentTypeRepository.Load(contentModel.ContentType);
                if (contentType == null)
                {
                    return null;
                }

                content = _contentFactory.CreateContent(contentType);
                if (content == null)
                {
                    return null;
                }
            }
            else
            {
                content = _contentRepository.Get<IContent>(link);

                var readOnly = content as IReadOnly;
                if (readOnly == null)
                {
                    return null;
                }
                content = readOnly.CreateWritableClone() as IContent;
            }

            if (content == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(contentModel.Name))
            {
                content.Name = contentModel.Name;
            }

            var languageData = content as ILocalizable;
            if (languageData != null && !string.IsNullOrEmpty(contentModel.Language))
            {
                languageData.Language = CultureInfo.GetCultureInfo(contentModel.Language);
            }

            var versionable = content as IVersionable;
            if (versionable != null)
            {
                if (contentModel.StartPublish.HasValue)
                {
                    versionable.StartPublish = contentModel.StartPublish;
                }

                if (contentModel.StopPublish.HasValue)
                {
                    versionable.StopPublish = contentModel.StopPublish;
                }

                if (!string.IsNullOrEmpty(contentModel.Status))
                {
                    VersionStatus status;
                    if (Enum.TryParse(contentModel.Status, out status))
                    {
                        versionable.Status = status;
                    }
                }
            }

            var changeTracking = content as IChangeTrackable;
            if (changeTracking != null)
            {
                if (contentModel.Created.HasValue)
                {
                    changeTracking.Created = contentModel.Created.Value;
                }

                if (!string.IsNullOrEmpty(contentModel.CreatedBy))
                {
                    changeTracking.CreatedBy = contentModel.CreatedBy;
                }

                if (contentModel.Modified.HasValue)
                {
                    changeTracking.Changed = contentModel.Modified.Value;
                }

                if (!string.IsNullOrEmpty(contentModel.ModifiedBy))
                {
                    changeTracking.ChangedBy = contentModel.ModifiedBy;
                }
            }

            var routeable = content as IRoutable;
            if (routeable != null && !string.IsNullOrEmpty(contentModel.UrlSegment))
            {
                routeable.RouteSegment = contentModel.UrlSegment;
            }

            var categorizable = content as ICategorizable;
            if (categorizable != null && contentModel.Categories != null && contentModel.Categories.Any())
            {
                categorizable.Category = new CategoryList(_categoryRepository.GetRoot()
                    .Categories
                    .Where(x => contentModel.Categories.Contains(x.Name))
                    .Select(x => x.ID));
            }

            foreach (var property in contentModel.Properties)
            {
                if (!content.Property.Contains(property.Name))
                {
                    continue;
                }

                foreach (var handler in PropertyModelHandlers)
                {
                    if (!handler.CanHandleModel(property))
                    {
                        continue;
                    }
                    handler.SetValue(content.Property[property.Name], property);
                    break;
                }
            }
            return content;
        }
    }

    public class ContentModel
    {
        public ContentModelReference Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string ContentType { get; set; }
        
        public ContentModelReference ParentId { get; set; }
        public string UrlSegment { get; set; }
        public List<string> Categories { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? StartPublish { get; set; }
        public DateTime? StopPublish { get; set; }
        public string Status { get; set; }

        [JsonConverter(typeof(PropertyModelConverter))]
        public List<IPropertyModel> Properties { get; set; } = new List<IPropertyModel>();
    }

    public class ContentModelReference
    {
        public int? Id { get; set; }
        public Guid? Guid { get; set; }
        public string ExternalId { get; set; }
        public int? VersionId { get; set; }
        public string ProviderName { get; set; }
    }
    
    public class ContentAreaItemModel
    {
        public ContentModel Model { get; set; }
        public string ContentGroup { get; set; }
        public List<string> AllowedRoles { get; set; }
        public string DisplayOption { get; set; }
        public string Tag { get; set; }
    }


    public class LinkItemNode
    {
        public string Href { get; set; }
        public string Title { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }
    }

    public sealed class ContentAreaPropertyModel : PropertyModel<List<ContentAreaItemModel>, PropertyContentArea>
    {
        //private Injected<ContentFragmentFactory> _contentFragmentFactory = default(Injected<ContentFragmentFactory>);
        //private Injected<ISecuredFragmentMarkupGeneratorFactory> _markupGeneratorFactory = default(Injected<ISecuredFragmentMarkupGeneratorFactory>);
        private Injected<Serializer> _serializer = default(Injected<Serializer>);

        public ContentAreaPropertyModel() { }

        public ContentAreaPropertyModel(PropertyContentArea propertyContentArea)
        {
            var contentArea = propertyContentArea.Value as ContentArea;
            Name = propertyContentArea.Name;
            Property = propertyContentArea;
            Value = contentArea?.Items.Select(x => new ContentAreaItemModel
            {

                Model = _serializer.Service.TransformContent(x.GetContent()),
                AllowedRoles = x.AllowedRoles?.ToList() ?? new List<string>(),
                ContentGroup = x.ContentGroup,
                DisplayOption = x.RenderSettings.ContainsKey(ContentFragment.ContentDisplayOptionAttributeName)
                    ? x.RenderSettings[ContentFragment.ContentDisplayOptionAttributeName].ToString() : ""
            }).ToList() ?? new List<ContentAreaItemModel>();
           
        }

        //public override bool SetValue(PropertyData propertyData)
        //{
        //    var area = new ContentArea();
        //    foreach (var contentAreaItemModel in Value)
        //    {
        //        var link = contentAreaItemModel.Id.GetContentReference();
        //        if (ContentReference.IsNullOrEmpty(link))
        //        {
        //            continue;
        //        }

        //        var markupGenerator = _markupGeneratorFactory.Service.CreateSecuredFragmentMarkupGenerator();
        //        markupGenerator.ContentGroup = contentAreaItemModel.ContentGroup;
        //        if (contentAreaItemModel.AllowedRoles != null && contentAreaItemModel.AllowedRoles.Any())
        //        {
        //            markupGenerator.RoleSecurityDescriptor.RoleIdentities = contentAreaItemModel.AllowedRoles;
        //        }

        //        var renderSetings = new Dictionary<string, object>();
        //        if (!string.IsNullOrEmpty(contentAreaItemModel.DisplayOption))
        //        {
        //            renderSetings[ContentFragment.ContentDisplayOptionAttributeName] =
        //                contentAreaItemModel.DisplayOption;
        //        }
        //        var fragement = _contentFragmentFactory.Service.CreateContentFragment(link, 
        //            contentAreaItemModel.Id.Guid ?? Guid.Empty, 
        //            contentAreaItemModel.Tag,
        //            markupGenerator,
        //            renderSetings);
        //        area.Items.Add(new ContentAreaItem(fragement));
        //    }
        //    propertyData.Value = area;
        //    return true;
        //}
    }

    public sealed class NumberPropertyModel : PropertyModel<int?, PropertyNumber>
    {
        public NumberPropertyModel() { }

        public NumberPropertyModel(PropertyNumber propertyNumber)
        {
            Name = propertyNumber.Name;
            Value = propertyNumber.Number;
            Property = propertyNumber;
        }

    }

    public sealed class BooleanPropertyModel : PropertyModel<bool?, PropertyBoolean>
    {
        public BooleanPropertyModel() { }

        public BooleanPropertyModel(PropertyBoolean propertyBoolean)
        {
            Name = propertyBoolean.Name;
            Value = propertyBoolean.Boolean;
            Property = propertyBoolean;
        }
        
    }

    public sealed class CategoryPropertyModel : PropertyModel<List<string>, PropertyCategory>
    {
        private Injected<CategoryRepository> _categoryRepository = new Injected<CategoryRepository>();

        public CategoryPropertyModel() { }

        public CategoryPropertyModel(PropertyCategory propertyCategory)
        {
            Name = propertyCategory.Name;
            Value = propertyCategory.Category.Select(x => _categoryRepository.Service.Get(x).Name)
                .ToList();
            Property = propertyCategory;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            propertyData.Value = new CategoryList(_categoryRepository.Service.GetRoot()
                .Categories
                .Where(x => Value.Contains(x.Name))
                .Select(x => x.ID));
            return true;
        }
    }

    public sealed class ContentReferencePropertyModel : PropertyModel<ContentModelReference, PropertyContentReference>
    {
        public ContentReferencePropertyModel() { }

        public ContentReferencePropertyModel(PropertyContentReference propertyContentReference)
        {
            Name = propertyContentReference.Name;
            Value = ContentReference.IsNullOrEmpty(propertyContentReference.ContentLink) ? null : new ContentModelReference
            {
                Id = propertyContentReference.ID,
                VersionId = propertyContentReference.WorkID,
                Guid = propertyContentReference.GuidValue,
                ProviderName = propertyContentReference.ProviderName
            };
            Property = propertyContentReference;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            var value =  Value?.GetContentReference() ?? ContentReference.EmptyReference;
            if (ContentReference.IsNullOrEmpty(value))
            {
                return false;
            }
            propertyData.Value = value;
            return true;
        }
    }

    public sealed class DateTimePropertyModel : PropertyModel<DateTime?, PropertyDate>
    {
        public DateTimePropertyModel() { }

        public DateTimePropertyModel(PropertyDate propertyDate)
        {
            Name = propertyDate.Name;
            Value = propertyDate.Date;
            Property = propertyDate;
        }
    }

    public sealed class FloatPropertyModel : PropertyModel<double?, PropertyFloatNumber>
    {
        public FloatPropertyModel() { }

        public FloatPropertyModel(PropertyFloatNumber propertyFloatNumber)
        {
            Name = propertyFloatNumber.Name;
            Value = propertyFloatNumber.FloatNumber;
            Property = propertyFloatNumber;
        }
    }

    public class LongStringPropertyModel : PropertyModel<string, PropertyLongString>
    {
        public LongStringPropertyModel() { }

        public LongStringPropertyModel(PropertyLongString propertyLongString)
        {
            Name = propertyLongString.Name;
            Value = propertyLongString.ToString();
            Property = propertyLongString;
        }
    }

    public class PageTypePropertyModel : PropertyModel<string, PropertyPageType>
    {
        public PageTypePropertyModel() { }

        public PageTypePropertyModel(PropertyPageType propertyPageType)
        {
            Name = propertyPageType.Name;
            Value = propertyPageType.PageTypeName;
            Property = propertyPageType;
        }
    }

    public class StringPropertyModel : PropertyModel<string, PropertyString>
    {
        public StringPropertyModel() { }

        public StringPropertyModel(PropertyString propertyString)
        {
            Name = propertyString.Name;
            Value = propertyString.ToString();
            Property = propertyString;
        }
    }

    public class WeekdayPropertyModel : PropertyModel<string, PropertyWeekDay>
    {
        public WeekdayPropertyModel() { }

        public WeekdayPropertyModel(PropertyWeekDay propertyWeekDay)
        {
            Name = propertyWeekDay.Name;
            Value = ((Weekday)propertyWeekDay.Value).ToString();
            Property = propertyWeekDay;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            Weekday weekday;
            if (!Enum.TryParse(Value, out weekday))
            {
                return false;
            }
            propertyData.Value = weekday;
            return true;
        }
    }

    public class VirtualLinkPropertyModel : PropertyModel<string, PropertyVirtualLink>
    {
        public VirtualLinkPropertyModel() { }

        public VirtualLinkPropertyModel(PropertyVirtualLink propertyVirtualLink)
        {
            Name = propertyVirtualLink.Name;
            Value = propertyVirtualLink.ToString();
            Property = propertyVirtualLink;
        }
    }

    public class UrlPropertyModel : PropertyModel<string, PropertyUrl>
    {
        public UrlPropertyModel() { }

        public UrlPropertyModel(PropertyUrl propertyUrl)
        {
            Name = propertyUrl.Name;
            Value = propertyUrl.ToString();
            Property = propertyUrl;
        }
    }

    public class SortOrderPropertyModel : PropertyModel<int?, PropertySortOrder>
    {
        public SortOrderPropertyModel() { }

        public SortOrderPropertyModel(PropertySortOrder propertySortOrder)
        {
            Name = propertySortOrder.Name;
            Value = propertySortOrder.Number;
            Property = propertySortOrder;
        }
    }

    public class SelectorPropertyModel : PropertyModel<List<string>, PropertySelector>
    {
        public SelectorPropertyModel() { }

        public SelectorPropertyModel(PropertySelector propertySelector)
        {
            Name = propertySelector.Name;
            Value = string.IsNullOrEmpty(propertySelector.ToString()) ? new List<string>() :
                propertySelector.ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            Property = propertySelector;
        }
    }

    public class LinkCollectionPropertyModel : PropertyModel<List<LinkItemNode>, PropertyLinkCollection>
    {
        public LinkCollectionPropertyModel() { }

        public LinkCollectionPropertyModel(PropertyLinkCollection propertyLinkCollection)
        {
            Name = propertyLinkCollection.Name;
            Value = propertyLinkCollection.Links == null || !propertyLinkCollection.Links.Any() ?
                new List<LinkItemNode>()  : 
                propertyLinkCollection.Links.Select(x => new LinkItemNode
                {
                    Text = x.Text,
                    Href = x.Href,
                    Target = x.Target,
                    Title = x.Title
                }).ToList();
            Property = propertyLinkCollection;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            if (Value == null || !Value.Any())
            {
                return false;
            }

            propertyData.Value = new LinkItemCollection(Value.Select(x => new LinkItem
            {
                Href = x.Href,
                Text = x.Text,
                Target = x.Target,
                Title = x.Title
            }));
            return true;
        }
    }

    public class LanguagePropertyModel : PropertyModel<string, PropertyLanguage>
    {
        public LanguagePropertyModel() { }

        public LanguagePropertyModel(PropertyLanguage propertyLanguage)
        {
            Name = propertyLanguage.Name;
            Value = propertyLanguage.ToString();
            Property = propertyLanguage;
        }
    }

    public class ImageUrlPropertyModel : PropertyModel<string, PropertyImageUrl>
    {
        public ImageUrlPropertyModel() { }

        public ImageUrlPropertyModel(PropertyImageUrl propertyImageUrl)
        {
            Name = propertyImageUrl.Name;
            Value = propertyImageUrl.ToString();
            Property = propertyImageUrl;
        }
    }

    public class FramePropertyModel : PropertyModel<string, PropertyFrame>
    {
        public FramePropertyModel() { }

        public FramePropertyModel(PropertyFrame propertyFrame)
        {
            Name = propertyFrame.Name;
            Value = propertyFrame.ToString();
            Property = propertyFrame;
        }
    }

    public class FileSortOrderPropertyModel : PropertyModel<int?, PropertyFileSortOrder>
    {
        public FileSortOrderPropertyModel() { }

        public FileSortOrderPropertyModel(PropertyFileSortOrder propertyFileSortOrder)
        {
            Name = propertyFileSortOrder.Name;
            Value = propertyFileSortOrder.Number;
            Property = propertyFileSortOrder;
        }
    }

    public class DropDownListPropertyModel : PropertyModel<string, PropertyDropDownList>
    {
        public DropDownListPropertyModel() { }

        public DropDownListPropertyModel(PropertyDropDownList propertyDropDownList)
        {
            Name = propertyDropDownList.Name;
            Value = propertyDropDownList.ToString();
            Property = propertyDropDownList;
        }
    }

    public class DocumentUrlPropertyModel : PropertyModel<string, PropertyDocumentUrl>
    {
        public DocumentUrlPropertyModel() { }

        public DocumentUrlPropertyModel(PropertyDocumentUrl propertyDocumentUrl)
        {
            Name = propertyDocumentUrl.Name;
            Value = propertyDocumentUrl.ToString();
            Property = propertyDocumentUrl;
        }
    }

    public class ContentReferenceListPropertyModel : PropertyModel<List<ContentModelReference>, PropertyContentReferenceList>
    {
        private Injected<IPermanentLinkMapper> _linkMapper = default(Injected<IPermanentLinkMapper>);

        public ContentReferenceListPropertyModel()
        {
        }

        public ContentReferenceListPropertyModel(PropertyContentReferenceList propertyContentReferenceList)
        {
            Name = propertyContentReferenceList.Name;
            Value = propertyContentReferenceList.List == null || !propertyContentReferenceList.List.Any() ? 
                null : 
                propertyContentReferenceList.List.Select(x => new ContentModelReference
                {
                    Id = x.ID,
                    VersionId = x.WorkID,
                    Guid = _linkMapper.Service.Find(x)?.Guid ?? Guid.Empty,
                    ProviderName = x.ProviderName
                }).ToList();
            Property = propertyContentReferenceList;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            if (Value == null || !Value.Any())
            {
                return false;
            }

            propertyData.Value = Value.Select(reference => reference.GetContentReference()).Where(link => !ContentReference.IsNullOrEmpty(link)).ToList();
            return true;
        }
    }

    public class CheckboxListPropertyModel : PropertyModel<List<string>, PropertyCheckBoxList>
    {
        public CheckboxListPropertyModel() { }

        public CheckboxListPropertyModel(PropertyCheckBoxList propertyCheckBoxList)
        {
            Name = propertyCheckBoxList.Name;
            Value = string.IsNullOrEmpty(propertyCheckBoxList.ToString()) ? new List<string>() :
                propertyCheckBoxList.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Property = propertyCheckBoxList;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            if (Value == null || !Value.Any())
            {
                return false;
            }

            propertyData.Value = string.Join(",", Value);
            return true;
        }
    }

    public class BlobPropertyModel : PropertyModel<string, PropertyBlob>
    {
        public BlobPropertyModel() { }

        public BlobPropertyModel(PropertyBlob propertyBlob)
        {
            Name = propertyBlob.Name;
            Value = propertyBlob.ToString();
            Property = propertyBlob;
        }
    }

    public class AppSettingsMultiplePropertyModel : PropertyModel<List<string>, PropertyAppSettingsMultiple>
    {
        public AppSettingsMultiplePropertyModel() { }

        public AppSettingsMultiplePropertyModel(PropertyAppSettingsMultiple propertyAppSettingsMultiple)
        {
            Name = propertyAppSettingsMultiple.Name;
            Value = string.IsNullOrEmpty(propertyAppSettingsMultiple.ToString()) ? new List<string>() :
                propertyAppSettingsMultiple.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Property = propertyAppSettingsMultiple;
        }

        public override bool SetValue(PropertyData propertyData)
        {
            if (Value == null || !Value.Any())
            {
                return false;
            }

            propertyData.Value = string.Join(",", Value);
            return true;
        }
    }

    public class AppSettingsPropertyModel : PropertyModel<string, PropertyAppSettings>
    {
        public AppSettingsPropertyModel() { }

        public AppSettingsPropertyModel(PropertyAppSettings propertyAppSettings)
        {
            Name = propertyAppSettings.Name;
            Value = propertyAppSettings.ToString();
            Property = propertyAppSettings;
        }
    }

    public class XHtmlPropertyModel : PropertyModel<string, PropertyXhtmlString>
    {
        public XHtmlPropertyModel() { }

        public XHtmlPropertyModel(PropertyXhtmlString propertyXhtmlString)
        {
            Name = propertyXhtmlString.Name;
            Value = propertyXhtmlString.ToString();
            Property = propertyXhtmlString;
        }
    }

    public class XFormPropertyModel : PropertyModel<string, PropertyXForm>
    {
        public XFormPropertyModel() { }

        public XFormPropertyModel(PropertyXForm propertyXForm)
        {
            Name = propertyXForm.Name;
            Value = propertyXForm.ToString();
            Property = propertyXForm;
        }
    }

    public class BlockPropertyModel<T> : PropertyModel<List<IPropertyModel>, PropertyBlock<T>> where T : BlockData
    {
        private readonly Injected<Serializer> _serializer = default(Injected<Serializer>);

        public BlockPropertyModel()
        {
            
        }

        public BlockPropertyModel(PropertyBlock<T> propertyBlock)
        {
            Name = propertyBlock.Name;
            Property = propertyBlock;
            var block = propertyBlock.Block;
            if (block == null)
            {
                return;
            }
            var properties = new List<IPropertyModel>();
            foreach (var property in block.Property)
            {
                foreach (var handler in _serializer.Service.PropertyModelHandlers)
                {
                    if (!handler.CanHandleProperty(property))
                    {
                        continue;
                    }
                    properties.Add(handler.GetValue(property));
                    break;
                }
            }
            Value = properties;
        }

        [JsonConverter(typeof(PropertyModelConverter))]
        public override List<IPropertyModel> Value { get; set; }

        public override bool SetValue(PropertyData propertyData)
        {
            var propertyBlock = propertyData as PropertyBlock<BlockData>;
            if (propertyBlock == null)
            {
                return false;
            }
            if (Value == null || !Value.Any() || propertyBlock.Block == null)
            {
                return false;
            }
            foreach (var property in Value)
            {
                if (!propertyBlock.Block.Property.Contains(property.Name))
                {
                    continue;
                }

                foreach (var handler in _serializer.Service.PropertyModelHandlers)
                {
                    if (!handler.CanHandleModel(property))
                    {
                        continue;
                    }
                    handler.SetValue(propertyBlock.Block.Property[property.Name], property);
                    break;
                }
            }
            return true;
        }
    }

    public abstract class PropertyModel<TValue, TType> : IPropertyModel<TType> where TType : PropertyData
    {
        protected PropertyModel()
        {
            var type = GetType().Name;
            Type = type.Contains("`") ? type.Remove(type.IndexOf("`")) : type;
        }
        public string Name { get; set; }
        public virtual TValue Value { get; set; }
        [JsonIgnore]
        public TType Property { get; set; }
        public string Type { get; set; }

        public virtual bool SetValue(PropertyData propertyData)
        {
            if (Value == null)
            {
                return false;
            }
            propertyData.Value = Value;
            return true;
        }
    }

    public interface IPropertyModel<TType> : IPropertyModel where TType : PropertyData
    {
        TType Property { get; set; }
    }

    public interface IPropertyModel
    {
        string Name { get; set; }
        string Type { get; set; }
        bool SetValue(PropertyData propertyData);
    }

    public interface IPropertyModelHandler
    {
        int SortOrder { get; }
        List<TypeModel> ModelTypes { get; }
        bool CanHandleModel(IPropertyModel propertyModel);
        bool CanHandleProperty(PropertyData propertyData);
        bool SetValue(PropertyData propertyData, IPropertyModel propertyModel);
        IPropertyModel GetValue(PropertyData propertyData);
    }

    public class TypeModel
    {
        public Type PropertyType { get; set; }
        public Type ModelType { get; set; }
        public string ModelTypeString { get; set; }
    }

    [ServiceConfiguration(ServiceType = typeof(IPropertyModelHandler), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DefaultPropertyModelHandler : IPropertyModelHandler
    {
        private static List<TypeModel> _cachedProperties;

        public DefaultPropertyModelHandler()
        {
            _cachedProperties = new List<TypeModel>
            {
                new TypeModel { ModelType = typeof(AppSettingsPropertyModel), ModelTypeString = nameof(AppSettingsPropertyModel), PropertyType = typeof(PropertyAppSettings) },
                new TypeModel { ModelType = typeof(AppSettingsMultiplePropertyModel), ModelTypeString = nameof(AppSettingsMultiplePropertyModel), PropertyType = typeof(PropertyAppSettingsMultiple) },
                new TypeModel { ModelType = typeof(BlobPropertyModel), ModelTypeString = nameof(BlobPropertyModel), PropertyType = typeof(PropertyBlob) },
                new TypeModel { ModelType = typeof(BooleanPropertyModel), ModelTypeString = nameof(BooleanPropertyModel), PropertyType = typeof(PropertyBoolean) },
                new TypeModel { ModelType = typeof(CategoryPropertyModel), ModelTypeString = nameof(CategoryPropertyModel), PropertyType = typeof(PropertyCategory) },
                new TypeModel { ModelType = typeof(CheckboxListPropertyModel), ModelTypeString = nameof(CheckboxListPropertyModel), PropertyType = typeof(PropertyCheckBoxList) },
                new TypeModel { ModelType = typeof(ContentAreaPropertyModel), ModelTypeString = nameof(ContentAreaPropertyModel), PropertyType = typeof(PropertyContentArea) },
                new TypeModel { ModelType = typeof(ContentReferencePropertyModel), ModelTypeString = nameof(ContentReferencePropertyModel), PropertyType = typeof(PropertyContentReference) },
                new TypeModel { ModelType = typeof(ContentReferenceListPropertyModel), ModelTypeString = nameof(ContentReferenceListPropertyModel), PropertyType = typeof(PropertyContentReferenceList) },
                new TypeModel { ModelType = typeof(DateTimePropertyModel), ModelTypeString = nameof(DateTimePropertyModel), PropertyType = typeof(PropertyDate) },
                new TypeModel { ModelType = typeof(DocumentUrlPropertyModel), ModelTypeString = nameof(DocumentUrlPropertyModel), PropertyType = typeof(PropertyDocumentUrl) },
                new TypeModel { ModelType = typeof(DropDownListPropertyModel), ModelTypeString = nameof(DropDownListPropertyModel), PropertyType = typeof(PropertyDropDownList) },
                new TypeModel { ModelType = typeof(FileSortOrderPropertyModel), ModelTypeString = nameof(FileSortOrderPropertyModel), PropertyType = typeof(PropertyFileSortOrder) },
                new TypeModel { ModelType = typeof(FloatPropertyModel), ModelTypeString = nameof(FloatPropertyModel), PropertyType = typeof(PropertyFloatNumber) },
                new TypeModel { ModelType = typeof(FramePropertyModel), ModelTypeString = nameof(FramePropertyModel), PropertyType = typeof(PropertyFrame) },
                new TypeModel { ModelType = typeof(ImageUrlPropertyModel), ModelTypeString = nameof(ImageUrlPropertyModel), PropertyType = typeof(PropertyImageUrl) },
                new TypeModel { ModelType = typeof(LanguagePropertyModel), ModelTypeString = nameof(LanguagePropertyModel), PropertyType = typeof(PropertyLanguage) },
                new TypeModel { ModelType = typeof(LinkCollectionPropertyModel), ModelTypeString = nameof(LinkCollectionPropertyModel), PropertyType = typeof(PropertyLinkCollection) },
                new TypeModel { ModelType = typeof(LongStringPropertyModel), ModelTypeString = nameof(LongStringPropertyModel), PropertyType = typeof(PropertyLongString) },
                new TypeModel { ModelType = typeof(NumberPropertyModel), ModelTypeString = nameof(NumberPropertyModel), PropertyType = typeof(PropertyNumber) },
                new TypeModel { ModelType = typeof(PageTypePropertyModel), ModelTypeString = nameof(PageTypePropertyModel), PropertyType = typeof(PropertyPageType) },
                new TypeModel { ModelType = typeof(SelectorPropertyModel), ModelTypeString = nameof(SelectorPropertyModel), PropertyType = typeof(PropertySelector) },
                new TypeModel { ModelType = typeof(SortOrderPropertyModel), ModelTypeString = nameof(SortOrderPropertyModel), PropertyType = typeof(PropertySortOrder) },
                new TypeModel { ModelType = typeof(StringPropertyModel), ModelTypeString = nameof(StringPropertyModel), PropertyType = typeof(PropertyString) },
                new TypeModel { ModelType = typeof(UrlPropertyModel), ModelTypeString = nameof(UrlPropertyModel), PropertyType = typeof(PropertyUrl) },
                new TypeModel { ModelType = typeof(VirtualLinkPropertyModel), ModelTypeString = nameof(VirtualLinkPropertyModel), PropertyType = typeof(PropertyVirtualLink) },
                new TypeModel { ModelType = typeof(WeekdayPropertyModel), ModelTypeString = nameof(WeekdayPropertyModel), PropertyType = typeof(PropertyWeekDay) },
                new TypeModel { ModelType = typeof(XHtmlPropertyModel), ModelTypeString = nameof(XHtmlPropertyModel), PropertyType = typeof(PropertyXhtmlString) },
                new TypeModel { ModelType = typeof(XFormPropertyModel), ModelTypeString = nameof(XFormPropertyModel), PropertyType = typeof(PropertyXForm) }
            };
        }
        public int SortOrder { get; } = 0;

        public List<TypeModel> ModelTypes => _cachedProperties;

        public bool CanHandleModel(IPropertyModel propertyModel)
        {
            return propertyModel != null && _cachedProperties.Any(x => x.ModelTypeString.Equals(propertyModel.Type));
        }

        public bool CanHandleProperty(PropertyData propertyData)
        {
            return  _cachedProperties.Any(x => x.PropertyType == propertyData.GetType()) ||
                    propertyData.GetType().GetInterfaces().Any(x => x.IsAssignableFrom(typeof(PropertyBlock<BlockData>)));
        }

        public bool SetValue(PropertyData propertyData,
            IPropertyModel propertyModel)
        {
             return propertyModel.SetValue(propertyData);
        }

        public IPropertyModel GetValue(PropertyData propertyData)
        {
            var modelType = _cachedProperties.FirstOrDefault(x => x.PropertyType == propertyData.GetType());
            if (modelType != null)
            {
                return (IPropertyModel) Activator.CreateInstance(modelType.ModelType, propertyData);
            }
            
            if (propertyData.GetType().GetInterfaces().Any(x => x.IsAssignableFrom(typeof(PropertyBlock<BlockData>))))
            {
                
                var block = propertyData as IPropertyBlock;
                if (block == null)
                {
                    return null;
                }
                var generic = typeof(BlockPropertyModel<>);
                var typeArgs = new [] { block.BlockType };
                var model = Activator.CreateInstance(generic.MakeGenericType(typeArgs), propertyData);

                return (IPropertyModel)model;
            }
            return null;
            
        }
    }

    public static class ContentModelReferenceExtensions
    {
        private static Injected<IPermanentLinkMapper> _linkMapper = default(Injected<IPermanentLinkMapper>);
        private static Injected<IdentityMappingService> _identityMappingService = default(Injected<IdentityMappingService>);

        public static ContentReference GetContentReference(this ContentModelReference reference)
        {
            if (reference.Id.HasValue && reference.Id.Value != 0)
            {
                if (reference.VersionId.HasValue)
                {
                    return !string.IsNullOrEmpty(reference.ProviderName) ?
                        new ContentReference(reference.Id.Value, reference.VersionId.Value, reference.ProviderName) :
                        new ContentReference(reference.Id.Value, reference.VersionId.Value);
                }
                return new ContentReference(reference.Id.Value);
            }

            if (reference.Guid.HasValue && reference.Guid.Value != Guid.Empty)
            {
                var linkedMapper = _linkMapper.Service.Find(reference.Guid.Value);
                if (linkedMapper != null && !ContentReference.IsNullOrEmpty(linkedMapper.ContentReference))
                {
                    return linkedMapper.ContentReference;
                }
            }

            if (!string.IsNullOrEmpty(reference.ExternalId))
            {
                var mapped = _identityMappingService.Service.Get(
                    MappedIdentity.ConstructExternalIdentifier(string.IsNullOrEmpty(reference.ProviderName) ?
                            "default" :
                            reference.ProviderName,
                        reference.ExternalId));
                if (mapped != null && !ContentReference.IsNullOrEmpty(mapped.ContentLink))
                {
                    return mapped.ContentLink;
                }
            }

            return ContentReference.EmptyReference;
        }

        public static ContentModelReference GetContentModelReference(this IContent content)
        {
            return new ContentModelReference
            {
                Id = content.ContentLink?.ID,
                Guid = content.ContentGuid,
                VersionId = content.ContentLink?.WorkID,
                ProviderName = content.ContentLink?.ProviderName
            };
        }

        public static ContentModelReference GetContentModelReference(this ContentReference contentReference)
        {
            return new ContentModelReference
            {
                Id = contentReference.ID,
                Guid = _linkMapper.Service.Find(contentReference)?.Guid,
                VersionId = contentReference.WorkID,
                ProviderName = contentReference.ProviderName
            };
        }
    }

    public class PropertyModelConverter : JsonConverter
    {
        private readonly Injected<Serializer> _serializer = default(Injected<Serializer>);

        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<IPropertyModel>);
        }
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            
            var models = new List<IPropertyModel>();
            var array = JArray.Load(reader);

            foreach (var model in array.Children<JObject>())
            {
                var type = model["Type"].Value<string>();
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }

                var modelType = _serializer.Service.ModelTypes.FirstOrDefault(x => x.ModelTypeString.Equals(type));
                if (modelType == null && !type.Equals("BlockPropertyModel", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var propertyModel = Activator.CreateInstance(type.Equals("BlockPropertyModel", StringComparison.InvariantCultureIgnoreCase) ? 
                    typeof(BlockPropertyModel<BlockData>) : 
                    modelType.ModelType) as IPropertyModel;
                
                serializer.Populate(model.CreateReader(), propertyModel);
                models.Add(propertyModel);
            }
            return models;
        }
    }
}