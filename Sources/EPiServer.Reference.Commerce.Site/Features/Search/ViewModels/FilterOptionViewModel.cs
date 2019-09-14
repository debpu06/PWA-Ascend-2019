using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.Search.ViewModels
{
    [ModelBinder(typeof(FilterOptionViewModelBinder))]
    public class FilterOptionViewModel
    {
        public List<FacetGroupOption> FacetGroups { get; set; }
        public string SelectedFacet { get; set; }
        public IEnumerable<SelectListItem> Sorting { get; set; }
        public string Sort { get; set; } = "Position";
        public string SortDirection { get; set; } = "Asc";
        public int Page { get; set; }
        public string Q { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 15;
        public string ViewSwitcher { get; set; } = "Grid";
        public decimal Confidence { get; set; }

        public List<int> Pages
        {
            get
            {
                if (TotalCount == 0)
                {
                    return new List<int>();
                }
                var totalPages = (TotalCount + PageSize - 1) / PageSize;
                var pages = new List<int>();
                var startPage = Page > 2 ? Page - 2 : 1; 
                for (var page = startPage; page < Math.Min((totalPages >= 5 ? startPage + 5 : startPage + totalPages), totalPages + 1); page++)
                {
                    pages.Add(page);
                }
                return pages;
            }
        }
        public bool HighlightTitle { get; set; }
        public bool HighlightExcerpt { get; set; }
        public string SectionFilter { get; set; }
        public bool SearchProduct { get; set; }
        public bool SearchContent { get; set; }
        public bool SearchPdf { get; set; }
        public bool TrackData { get; set; } = true;
    }

}