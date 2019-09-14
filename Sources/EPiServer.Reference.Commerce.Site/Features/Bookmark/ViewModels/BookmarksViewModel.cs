using EPiServer.Reference.Commerce.Site.Features.Bookmark.Models;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.ViewModels
{
    public class BookmarksViewModel : ContentViewModel<BookmarksPage>
    {
        public List<BookmarkModel> Bookmarks { get; set; }
    }
}