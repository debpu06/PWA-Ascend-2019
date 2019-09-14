using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules
{
    public class PagingInfo
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalRecord { get; set; }

        public List<int> Pages
        {
            get
            {
                if (TotalRecord == 0)
                {
                    return new List<int>();
                }
                var totalPages = (TotalRecord + PageSize - 1) / PageSize;
                var pages = new List<int>();
                var startPage = PageNumber > 2 ? PageNumber - 2 : 1;
                for (var page = startPage; page < Math.Min((totalPages >= 5 ? startPage + 5 : startPage + totalPages), totalPages + 1); page++)
                {
                    pages.Add(page);
                }
                return pages;
            }
        }
    }
}