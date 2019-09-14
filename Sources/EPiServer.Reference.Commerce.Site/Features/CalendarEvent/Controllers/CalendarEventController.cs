using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.CalendarEvent.Pages;
using EPiServer.Reference.Commerce.Site.Features.CalendarEvent.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.CalendarEvent.Controllers
{
    public class CalendarEventController : PageController<CalendarEventPage>
    {
        public ActionResult Index(CalendarEventPage currentPage)
        {
            var model = new CalendarEventViewModel(currentPage);

            return View(model);
        }
    }
}