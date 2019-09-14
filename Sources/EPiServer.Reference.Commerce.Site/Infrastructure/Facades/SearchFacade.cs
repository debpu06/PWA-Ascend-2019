using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using StringCollection = System.Collections.Specialized.StringCollection;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Facades
{
    [ServiceConfiguration(typeof(SearchFacade), Lifecycle = ServiceInstanceScope.Singleton)]
    public class SearchFacade
    {
        public enum SearchProviderType
        {
            Lucene,
            Unknown
        }

        private SearchManager _searchManager;
        private SearchProviderType _searchProviderType;
        private bool _initialized;
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;

        public SearchFacade(IContentLoader contentLoader,
            ReferenceConverter referenceConverter)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
        }

        public virtual ISearchResults Search(CatalogEntrySearchCriteria criteria)
        {
            Initialize();
            return _searchManager.Search(criteria);
        }

        public virtual SearchProviderType GetSearchProvider()
        {
            Initialize();
            return _searchProviderType;
        }

        public virtual SearchFilter[] SearchFilters
        {
            get { return SearchFilterHelper.Current.SearchConfig.SearchFilters; }
        }

        public virtual StringCollection GetOutlinesForNode(string code)
        {
            var nodes = SearchFilterHelper.GetOutlinesForNode(code);
            if (nodes.Count == 0)
            {
                return nodes;
            }
            nodes[nodes.Count - 1] = nodes[nodes.Count - 1].Replace("*", "");
            return nodes;
        }

        public virtual string GetOutline(string nodeCode)
        {
            return GetOutlineForNode(nodeCode);
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _searchManager = new SearchManager(Mediachase.Commerce.Core.AppContext.Current.ApplicationName);
            _searchProviderType = LoadSearchProvider();
            _initialized = true;
        }

        private SearchProviderType LoadSearchProvider()
        {
            var element = SearchConfiguration.Instance.SearchProviders;
            if (element.Providers == null ||
                String.IsNullOrEmpty(element.DefaultProvider) ||
                String.IsNullOrEmpty(element.Providers[element.DefaultProvider].Type))
            {
                return SearchProviderType.Unknown;
            }

            var providerType = Type.GetType(element.Providers[element.DefaultProvider].Type);
            var baseType = Type.GetType("Mediachase.Search.Providers.Lucene.LuceneSearchProvider, Mediachase.Search.LuceneSearchProvider");
            if (providerType == null || baseType == null)
            {
                return SearchProviderType.Unknown;
            }
            if (providerType == baseType || providerType.IsSubclassOf(baseType))
            {
                return SearchProviderType.Lucene;
            }

            return SearchProviderType.Unknown;
        }

        private string GetOutlineForNode(string nodeCode)
        {
            if (string.IsNullOrEmpty(nodeCode))
            {
                return "";
            }
            var outline = nodeCode;
            var currentNode = _contentLoader.Get<NodeContent>(_referenceConverter.GetContentLink(nodeCode));
            var parent = _contentLoader.Get<CatalogContentBase>(currentNode.ParentLink);
            while (!ContentReference.IsNullOrEmpty(parent.ParentLink))
            {
                var catalog = parent as CatalogContent;
                if (catalog != null)
                {
                    outline = string.Format("{1}/{0}", outline, catalog.Name);
                }

                var parentNode = parent as NodeContent;
                if (parentNode != null)
                {
                    outline = string.Format("{1}/{0}", outline, parentNode.Code);
                }

                parent = _contentLoader.Get<CatalogContentBase>(parent.ParentLink);

            }
            return outline;
        }
    }
}