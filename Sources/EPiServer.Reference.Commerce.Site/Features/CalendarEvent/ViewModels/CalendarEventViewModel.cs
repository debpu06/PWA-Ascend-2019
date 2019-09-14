using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.CalendarEvent.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.CalendarEvent.ViewModels
{
    public class CalendarEventViewModel : ContentViewModel<CalendarEventPage>
    {
        public CalendarEventViewModel(CalendarEventPage calendarEventPage) : base(calendarEventPage)
        {
        }
    }
}