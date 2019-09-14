using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.CalendarEvent.Pages;


namespace EPiServer.Reference.Commerce.Site.Features.Folder.Pages
{
    [ContentType(DisplayName = "Calendar Event Folder Page", GUID = "4146cdca-742b-45cb-9ebb-b0cd3371ad31", Description = "A folder page allows only Calendar Event page type created", GroupName = "Calendar Event")]
    [AvailableContentTypes(Include = new[] { typeof(CalendarEventPage), typeof(CalendarEventFolderPage) })]
    [ImageUrl("~/content/icons/pages/container.png")]
    public class CalendarEventFolderPage : SitePageData
    {
    }
}