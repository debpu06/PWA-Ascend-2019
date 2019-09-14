using System;
using System.Collections.Generic;
using System.Web;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels
{
    public class FindSearchContentModel : ContentViewModel<FindPage>
    {
        public FindSearchContentModel()
        {
            
        }

        public FindSearchContentModel(FindPage currentPage) : base(currentPage)
        {
            
        }

        public UnifiedSearchResults Hits { get; set; }

        public string PublicProxyPath { get; set; }

        public string PageUrl { get; set; }

        public string GetSectionGroupUrl(string groupName)
        {
            string url = UriUtil.AddQueryString(HttpContext.Current.Request.RawUrl, "t", HttpContext.Current.Server.UrlEncode(groupName));
            url = UriUtil.AddQueryString(url, "p", "1");
            return url;
        }

        public int NumberOfHits => Hits.TotalMatching;

        public List<DateRange> PublishedDateRange
        {
            get
            {
                var dateRanges = new List<DateRange>()
                {
                    new DateRange { From = DateTime.Now.AddDays(-1), To = DateTime.Now },
                    new DateRange { From = DateTime.Now.AddDays(-7), To = DateTime.Now.AddDays(-1) },
                    new DateRange { From = DateTime.Now.AddDays(-30), To = DateTime.Now.AddDays(-7) },
                    new DateRange { From = DateTime.Now.AddDays(-365), To = DateTime.Now.AddDays(-365) },
                    new DateRange { From = DateTime.Now.AddYears(-2), To = DateTime.Now.AddDays(-365) },
                };
                return dateRanges;
            }
        }

        public string SectionFilter => HttpContext.Current.Request.QueryString["t"] ?? string.Empty;

        //Retrieve the paging page from the query string parameter "p".
        //If no such parameter exists the user hasn't requested a specific
        //page so we default to the first (1).
        public int PagingPage
        {
            get
            {
                int pagingPage;
                if (!int.TryParse(HttpContext.Current.Request.QueryString["p"], out pagingPage))
                {
                    pagingPage = 1;
                }

                return pagingPage;
            }
        }

        //Calculate the number of paged result listings based on the
        //total number of hits and the PageSize.
        public int TotalPagingPages
        {
            get
            {
                if (CurrentContent.PageSize > 0)
                {
                    return 1 + (Hits.TotalMatching - 1) / CurrentContent.PageSize;
                }

                return 0;
            }
        }

        //Create URL for a specific paging page.
        public string GetPagingUrl(int pageNumber)
        {
            return UriUtil.AddQueryString(HttpContext.Current.Request.RawUrl, "p", pageNumber.ToString());
        }

        public string Query => (HttpContext.Current.Request.QueryString["search"] ?? string.Empty).Trim();

        public FilterOptionViewModel FilterOption { get; set; }
    }
}
