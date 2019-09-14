using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark
{
    public interface IBookmarksService
    {
        void Add(int contentId);
        void Remove(Guid contentGuid);
        List<BookmarkModel> Get();
    }
}
