using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.Pages
{
    [ContentType(DisplayName = "Bookmarks Page", GUID = "40E76908-6AA2-4CB7-8239-607D941DF3A6", Description = "This page displays list the different content that has been bookmarked belonging to an user", GroupName = "Content", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-28.png")]
    public class BookmarksPage : SitePageData
    {
    }
}