using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Tracking.Models;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Tracking;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Security;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Controllers
{
    public class TrackingBuilderPageController : PageController<TrackingBuilderPage>
    {
        private readonly Injected<IPageTrackingService> _pageTrackingService;
        private readonly Injected<IContentRepository> _contentRepository;

        public ActionResult Index(TrackingBuilderPage currentPage)
        {
            TrackingBuilderPageViewModel model = new TrackingBuilderPageViewModel(currentPage)
            {
                TrackingItems = currentPage.TrackingItems,
                ContentGuid = currentPage.ContentGuid.ToString(),
                CurrentUserEmail = SecurityContext.Current.CurrentUserProfile.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> InjectEvents(string userId, string pageContentLinkId)
        {
            Guid pageContentLink = new Guid(pageContentLinkId);
            TrackingBuilderPage page = _contentRepository.Service.Get<TrackingBuilderPage>(pageContentLink);
            if (page != null)
            {
                foreach (ContentAreaItem item in page.TrackingItems.Items)
                {
                    TrackingEventBlock eventData = _contentRepository.Service.Get<TrackingEventBlock>(item.ContentLink);

                    if (eventData != null)
                    {
                        await _pageTrackingService.Service.TrackEventInjection(HttpContext, eventData.DateAndTime, eventData.EventType, eventData.Description, userId, new PageVisitEvent());
                     }
                }
            }

            return RedirectToAction("Index");
        }
    }
}