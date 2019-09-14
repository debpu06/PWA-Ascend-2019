using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Find;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Api.Querying.Filters;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ProductFilters;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Blocks
{
    [ContentType(
        DisplayName = "Configurable Product List",
        GUID = "8BD1CF05-4980-4BA2-9304-C0EAF946DAD5",
        Description = "Configurable search block for all products, allows generic filtering.",
        GroupName = "Commerce")]
    [ImageUrl("~/content/icons/pages/search.png")]
    public class ProductSearchBlock : BlockData
    {
        private int _startingIndex = 0;

        [Display(Name = "Categories",
            Description = "Root categories to get products from, includes sub categories",
            GroupName = SystemTabNames.Content, Order = 5)]
        [AllowedTypes(typeof(NodeContent))]
        public virtual ContentArea Nodes { get; set; }

        [Display(Name = "Filters",
            Description = "Filters to apply to the search result",
            Order = 10)]
        [AllowedTypes(typeof(FilterBaseBlock))]
        public virtual ContentArea Filters { get; set; }

        [Display(Name = "Priority Products",
            Description = "Products to put first in the list.",
            Order = 20)]
        [AllowedTypes(typeof(EntryContentBase))]
        public virtual ContentArea PriorityProducts { get; set; }

        [Display(Order = 3,
            Name = "Search Term")]
        [CultureSpecific]
        public virtual string SearchTerm { get; set; }

        [Display(Order = 1,
            Name = "Heading")]
        [CultureSpecific]
        public virtual string Heading { get; set; }

        [Display(Name = "Number of Results",
            Description = "The number of products to show in the list. Default is 6.",
            Order = 9)]
        [CultureSpecific]
        public virtual int ResultsPerPage { get; set; }

        [Display(Order = 11,
            Name = "Min Price",
            Description = "The minimum price in the current market currency")]
        [CultureSpecific]
        public virtual int MinPrice { get; set; }

        [Display(Order = 13,
            Name = "Max Price",
            Description = "The maximum price in the current market currency")]
        [CultureSpecific]
        public virtual int MaxPrice { get; set; }


        public void SetIndex(int index)
        {
            _startingIndex = index;
        }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ResultsPerPage = 6;
        }

        public virtual CustomSearchResult GetSearchResults(string language)
        {
            var searchService = ServiceLocator.Current.GetInstance<ISearchService>();
            var filterOptions = new FilterOptionViewModel
            {
                Q = SearchTerm,
                PageSize = ResultsPerPage,
                Sort = string.Empty,
                FacetGroups = new List<FacetGroupOption>(),
                Page = 1
            };

            var filters = GetFilters();
            return searchService.SearchWithFilters(null, filterOptions, filters);
        }

        private IEnumerable<Filter> GetFilters()
        {
            var filters = new List<Filter>();
            if (Nodes?.FilteredItems != null && Nodes.FilteredItems.Any())
            {
                var searchFacade = ServiceLocator.Current.GetInstance<SearchFacade>();
                var nodes = Nodes.FilteredItems.Select(x => x.GetContent()).OfType<NodeContent>().ToList();
                var outlines = nodes.Select(x => searchFacade.GetOutline(x.Code)).ToList();
                var outlineFilters = outlines.Select(s => new PrefixFilter("Outline$$string.lowercase", s.ToLowerInvariant()))
                    .ToList();

                if (outlineFilters.Count == 1)
                {
                   filters.Add(outlineFilters.First());
                }
                else
                {
                    filters.Add(new OrFilter(outlineFilters.ToArray()));
                }
                
            }

            if (MinPrice > 0 || MaxPrice > 0)
            {
                var rangeFilter = RangeFilter.Create("DefaultPrice$$number",
                    MinPrice.ToString(),
                    MaxPrice == 0 ? double.MaxValue.ToString() : MaxPrice.ToString());
                    rangeFilter.IncludeUpper = true;
                filters.Add(rangeFilter);
            }

            if (Filters == null)
            {
                return filters;
            }
            foreach (var item in Filters.FilteredItems)
            {
                FilterBaseBlock filter = item.GetContent() as FilterBaseBlock;
                if (filter != null)
                {
                    filters.Add(filter.GetFilter());
                }
            }
            return filters;
        }
    }
}
