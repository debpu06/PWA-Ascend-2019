using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Tracking
{
    public interface IPageTrackingService
    {
        Task TrackEventInjection(HttpContextBase httpContext, DateTime date, string pageTypeDescription, string trackingValue, string email, PageVisitEvent payload);

        string GetUserTrackingId(string email);

        SwitchableUser GetUserTrackingObject(string email);
    }
}
