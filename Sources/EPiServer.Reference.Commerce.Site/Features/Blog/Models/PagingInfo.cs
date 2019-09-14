using System;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models
{
    /// <summary>
    /// Hold informations to support paging on template
    /// </summary>
    public class PagingInfo
    {
        public PagingInfo()
        {

        }
        public PagingInfo(int pageId, int pageSize, int pageIndex)
        {
            PageId = pageId;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }
        public int PageIndex { get; set; } = 1;
        public int PageId { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 5;
        public int PageCount
        {
            get
            {
                return (PageSize == -1 && TotalCount > 0) ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
}