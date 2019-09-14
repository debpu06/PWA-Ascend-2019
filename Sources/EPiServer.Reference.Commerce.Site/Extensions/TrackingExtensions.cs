using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Reference.Commerce.Site.Features.Tracking;
using EPiServer.ServiceLocation;
using EPiServer.Tracking.Core;
using System;
using System.Linq;
using System.Web;
using EPiServer.Personalization;
using EPiServer.Personalization.VisitorGroups;
using System.Collections.Generic;
using System.Web.Hosting;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Tracking.Cms;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class TrackingExtensions
    {
        private static Lazy<ITrackingService> _trackingService = new Lazy<ITrackingService>(() => ServiceLocator.Current.GetInstance<ITrackingService>());
        private static Lazy<IContentLoader> _contentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());

        public static void TrackBlock(this BlockData block, IContent page, HttpContextBase httpContext)
        {
            try
            {
                var trackingData = new TrackingData<BlockPerformance>
                {
                    EventType = typeof(BlockPerformance).Name,
                    User = GetUserData(httpContext),
                    Value = "Block viewed: '" + (block as IContent).Name + "' on page - '" + page.Name + "'",
                    PageUri = PageEditing.GetEditUrl(page.ContentLink),
                    Payload = new BlockPerformance
                    {
                        PageName = page.Name,
                        PageId = page.ContentLink.ID,
                        BlockId = (block as IContent).ContentLink.ID,
                        BlockName = (block as IContent).Name
                    }
                };

                _trackingService.Value.Track(trackingData, httpContext);
            }
            catch
            {
            }

        }

        public static void TrackVisitorGroups(this IContent page, HttpContextBase httpContext)
        {
            try
            {
                var startPage = _contentLoader.Value.Get<IContent>(ContentReference.StartPage) as BaseStartPage;
                if (startPage == null) return;

                if (string.IsNullOrEmpty(startPage.VisitorGroupsToTrack)) return;

                var matchedVGs = new List<string>();
                var vgHelper = new VisitorGroupHelper();
                foreach (var visitorGroup in startPage.VisitorGroupsToTrack.Split(",".ToCharArray()))
                {
                    if (vgHelper.IsPrincipalInGroup(httpContext.User, visitorGroup))
                    {
                        matchedVGs.Add(visitorGroup);
                    }
                }

                var trackData = new TrackingData<VisitorGroupTrackDataWrapper>
                {
                    Payload = new VisitorGroupTrackDataWrapper
                    {
                        Epi = new VisitorGroupTrackingData
                        {
                            IncludeVisitorGroups = matchedVGs, //matched group
                            ExcludeVisitorGroups = Enumerable.Empty<string>()
                        }
                    }
                };

                if (matchedVGs.Count > 0)
                {
                    var track = _trackingService.Value.TrackVisitorGroupAsync(trackData, httpContext);
                    track.Start();
                }
            }
            catch
            {
            }
        }

        public static void TrackImageView(this ImageData image, IContent page, HttpContextBase httpContext)
        {
            try
            {
                var trackingData = new TrackingData<ImageView>
                {
                    EventType = "Imagery",
                    User = GetUserData(httpContext),
                    Value = "Image viewed: '" + (image as IContent).Name + "' on page - '" + page.Name + "'",
                    PageUri = PageEditing.GetEditUrl(page.ContentLink),
                    Payload = new ImageView
                    {
                        PageName = page.Name,
                        PageId = page.ContentLink.ID,
                        ImageId = (image as IContent).ContentLink.ID,
                        ImageName = (image as IContent).Name
                    }
                };

                _trackingService.Value.Track(trackingData, httpContext);
            }
            catch
            {
            }
        }

        private static UserData GetUserData(HttpContextBase httpContext)
        {
            try
            {
                if (httpContext?.Request.Cookies["_epicampaign_trk"]?.Value != null)
                {
                    var email = HttpContext.Current.Request.Cookies["_epicampaign_trk"]?.Value;
                    return new UserData()
                    {
                        Email = email,
                        Name = email
                    };
                }

                EPiServerProfile current = EPiServerProfile.Current;
                if (!string.IsNullOrEmpty(current.Email))
                {
                    UserData data1 = new UserData
                    {
                        Name = (current != null) ? current.UserName : "",
                        Email = (current != null) ? current.Email : ""
                    };
                    return data1;
                }

                return null;
            }
            catch
            {
                return new UserData()
                {
                    Email = Personalization.EPiServerProfile.Current.Email,
                    Name = Security.PrincipalInfo.CurrentPrincipal.Identity.Name
                };
            }
        }
    }
}