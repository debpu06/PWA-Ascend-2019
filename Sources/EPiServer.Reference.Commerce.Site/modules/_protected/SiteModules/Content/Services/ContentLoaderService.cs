using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// This service is an abstraction layer on top of content and content repositories.
    /// It mostly can be used to load content data from repository
    /// </summary>
    [ServiceConfiguration(typeof(ContentLoaderService))]
    public class ContentLoaderService
    {
        private readonly IContentLoader _contentLoader;
        private readonly LanguageSelectorService _languageSelectorService;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly ReferenceConverter _referenceConverter;

        /// <summary>
        /// Initialize a new instance of <see cref="ContentLoaderService"/>
        /// </summary>
        public ContentLoaderService(IContentLoader contentLoader,
            LanguageSelectorService languageSelectorService,
            IContentTypeRepository contentTypeRepository, 
            ReferenceConverter referenceConverter)
        {
            _contentLoader = contentLoader;
            _languageSelectorService = languageSelectorService;
            _contentTypeRepository = contentTypeRepository;
            _referenceConverter = referenceConverter;
        }

        /// <summary>
        /// Get content item represented by the provided reference in given language. 
        /// This method supports fallback language
        /// i.e: if content in the given language does not exist , so the content in fallback language is returned. 
        /// </summary>        
        /// <param name="contentReference">The reference to the content.</param>
        /// <param name="language">The language of content to get</param>
        /// <returns>The requested content item, as the specified type.</returns>
        public virtual IContent Get(ContentReference contentReference, string language)
        {
            if (ContentReference.IsNullOrEmpty(contentReference))
            {
                return null;
            }

            var fallbackLanguageSelector = _languageSelectorService.CreateFallbackSelector(language);
            return _contentLoader.Get<IContent>(contentReference, fallbackLanguageSelector);
        }

        /// <summary>
        /// Get content item represented by the provided GUID in given language. 
        /// This method supports fallback language
        /// i.e: if content in the given language does not exist , so the content in fallback language is returned. 
        /// </summary>        
        /// <param name="guid">The reference to the content.</param>
        /// <param name="language">The language of content to get</param>
        /// <returns>The requested content item, as the specified type.</returns>
        public virtual IContent Get(Guid guid, string language)
        {
            if (guid == Guid.Empty)
            {
                return null;
            }

            var fallbackLanguageSelector = _languageSelectorService.CreateFallbackSelector(language);
            return _contentLoader.Get<IContent>(guid, fallbackLanguageSelector);
        }

        /// <summary>
		/// Get the children of the content represented by the provided reference in given language. 
		/// This method supports fallback language
        /// i.e: if content in the given language does not exist , so the content in fallback language is returned. 
		/// </summary>
		/// <param name="contentReference"></param>
		/// <param name="language"></param>
		/// <returns> all children of the given content </returns>
		public virtual IEnumerable<IContent> GetChildren(ContentReference contentReference, string language)
        {
            if (ContentReference.IsNullOrEmpty(contentReference))
            {
                return Enumerable.Empty<IContent>();
            }

            var fallbackLanguageSelector = _languageSelectorService.CreateFallbackSelector(language);
            return _contentLoader.GetChildren<IContent>(contentReference, fallbackLanguageSelector);
        }

        /// <summary>
        /// Get ancestors of the content represented by the provided reference in given language. 
        /// This method supports fallback language
        /// i.e: if content in the given language does not exist , so the content in fallback language is returned. 
        /// </summary>
        /// <param name="contentReference"></param>
        /// <param name="language"></param>
        /// <returns> all ancestors of the given content</returns>
        public virtual IEnumerable<IContent> GetAncestors(ContentReference contentReference, string language)
        {
            if (ContentReference.IsNullOrEmpty(contentReference))
            {
                return Enumerable.Empty<IContent>();
            }

            var ancestors = _contentLoader.GetAncestors(contentReference);

            var fallbackLanguageSelector = _languageSelectorService.CreateFallbackSelector(language);
            return _contentLoader.GetItems(ancestors.Select(x => x.ContentLink), fallbackLanguageSelector);
        }

        /// <summary>
        /// Get all content items that is represented by the provided references given the language. 
        /// This method supports fallback language
        /// i.e: if content in the given language does not exist , so the content in fallback language is returned. 
        /// </summary>
        /// <param name="contentReferences"></param>
        /// <param name="language"></param>
        /// <returns> A list of content for the specifed references </returns>
        public virtual IEnumerable<IContent> GetItemsWithFallback(IEnumerable<ContentReference> contentReferences, string language)
        {
            if (contentReferences == null || !contentReferences.Any())
            {
                return Enumerable.Empty<IContent>();
            }

            var fallbackLanguageSelector = _languageSelectorService.CreateFallbackSelector(language);
            return _contentLoader.GetItems(contentReferences, fallbackLanguageSelector);
        }

        /// <summary>
        /// Get all content items that is represented by the provided references in the given language. 
        /// For references associated with a specific version (that is where EPiServer.Core.ContentReference.WorkID  is set) 
        ///	the language is ignored and that version is returned. 
        ///	If contentReferences contain duplicate entries, this method returns all content including duplicated ones
        /// </summary>
        /// <param name="contentReferences"></param>
        /// <param name="language"></param>
        /// <returns> A list of content for the specifed references </returns>
        public virtual IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentReferences, CultureInfo language)
        {
            if (contentReferences == null || !contentReferences.Any())
            {
                return Enumerable.Empty<IContent>();
            }

            var contentList = new List<IContent>();
            foreach (var contentLink in contentReferences)
            {
                var content = this.Get(contentLink, language);
                if (content != null)
                {
                    contentList.Add(content);
                }
            }

            return contentList;
        }

        private IContent Get(ContentReference contentReference, CultureInfo language)
        {
            try
            {
                return _contentLoader.Get<IContent>(contentReference, language);
            }
            catch (ContentNotFoundException)
            {
                return null;
            }
        }

        public virtual IEnumerable<IContent> GetByContentType(int contentTypeId, string language)
        {
            var contentType = _contentTypeRepository.Load(contentTypeId);
            var catalogContent = typeof(CatalogContentBase).IsAssignableFrom(contentType.ModelType);
            var contentReferences = _contentLoader.GetDescendents(!catalogContent ? ContentReference.RootPage : _referenceConverter.GetRootLink());
            var contents = GetItemsWithFallback(contentReferences, language);
            return contents.Where(o => o.ContentTypeID == contentTypeId).ToList();
        }
    }
}