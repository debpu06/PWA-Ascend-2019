using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.ServiceLocation;
using EPiServer.Tracking.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Tracking
{
    [ServiceConfiguration(typeof(IPageTrackingService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PageTrackingService : IPageTrackingService
    {
        private readonly ITrackingService _trackingService;
        private static IContentLoader _contentLoader;

        public PageTrackingService(
            ITrackingService trackingService,
            IContentLoader contentLoader)
        {
            _trackingService = trackingService;
            _contentLoader = contentLoader;
        }

        public async Task TrackEventInjection(HttpContextBase httpContext, DateTime date, string trackingType, string trackingValue,
                                         string email, PageVisitEvent payload)
        {
            //get the name for the user, if in the quick access users
            SwitchableUser user = GetUserTrackingObject(email);
            string name = email;
            if (user != null)
            {
                name = user.FirstName + " " + user.LastName;
            }

            TrackingData<PageVisitEvent> trackingData = this.GetTrackingObject(trackingType, trackingValue, date, email, name, payload);

            if (trackingData == null)
            {
                return;
            }

            await _trackingService.Track<PageVisitEvent>(trackingData, httpContext);
        }

        public TrackingData<PageVisitEvent> GetTrackingObject(string eventType, string trackingValue, DateTime date, string email, string name, PageVisitEvent payload)
        {
            return new TrackingData<PageVisitEvent>()
            {
                EventType = eventType,
                EventTime = date,
                User = new UserData
                {
                    Email = email,
                    Name = name,
                },
                Value = trackingValue,
                Payload = payload
            };
        }

        public string GetUserTrackingId(string email)
        {
            BaseStartPage start = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            System.Collections.Generic.IList<SwitchableUser> users = start.QuickAccessLogins;

            if (users != null)
            {
                foreach (var user in users)
                {
                    if (user.UserName.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return user.UserTrackingCode;
                    }
                }
            }

            return Guid.NewGuid().ToString();
        }

        public SwitchableUser GetUserTrackingObject(string email)
        {
            BaseStartPage start = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            System.Collections.Generic.IList<SwitchableUser> users = start.QuickAccessLogins;

            if (users != null)
            {
                foreach (var user in users)
                {
                    if (user.UserName.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return user;
                    }
                }
            }

            return null;
        }
    }
}