using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Personalization;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Tracking;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore;
using EPiServer.Security;
using EPiServer.Tracking.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Controllers
{
    public class InsightPayloadBlockController : BlockController<InsightPayloadBlock>
    {
        private ITrackingService _trackingService;
        private IContextModeResolver _contextModeResolver;
        private ProfileStoreServiceSync _profileStoreService;

        public InsightPayloadBlockController(
            ITrackingService trackingService,
            IContextModeResolver contextModeResolver)
        {
            _trackingService = trackingService;
            _contextModeResolver = contextModeResolver;
            _profileStoreService = new ProfileStoreServiceSync();
        }

        public override ActionResult Index(InsightPayloadBlock currentBlock)
        {
            if (_contextModeResolver.CurrentMode.EditOrPreview())
            {
                return PartialView(currentBlock);
            }

            var currentUser = GetUserData();

            if (string.IsNullOrEmpty(currentUser?.Email))
                return PartialView(currentBlock);

            var query = $"?$filter=Scope eq 'default' and Info.Email eq '{currentUser.Email}'";
            var profile = _profileStoreService.GetProfiles(query);
            var model = profile.ProfileStoreList.FirstOrDefault();

            if (model == null)
            {
                string deviceId = Guid.NewGuid().ToString();

                var payloadData = new Dictionary<string, string>();
                payloadData.Add(currentBlock.ProfileValueKey, currentBlock.ProfileValueValue);
                ProfileStoreModel newUser = new ProfileStoreModel()
                {
                    Name = currentUser.Email,
                    ProfileManager = string.Empty,
                    FirstSeen = DateTime.Now,
                    LastSeen = DateTime.Now,
                    Visits = 1,
                    Info = new ProfileStoreInformation()
                    {
                        Email = currentUser.Email
                    },
                    ContactInformation = new List<string>()
                    {
                        "Mailable",
                        "Known"
                    },
                    DeviceIds = new List<object> { deviceId },
                    Scope = "default",
                    Payload = payloadData
                };

                _profileStoreService.EditOrCreateProfile("default", newUser);
            }
            else
            {
                if (model.Payload != null)
                {
                    if (model.Payload.ContainsKey(currentBlock.ProfileValueKey))
                    {
                        // update
                        model.Payload[currentBlock.ProfileValueKey] = currentBlock.ProfileValueValue;
                    }
                    else
                    {
                        // add
                        model.Payload.Add(currentBlock.ProfileValueKey, currentBlock.ProfileValueValue);
                    }
                }
                else
                {
                    model.Payload = new Dictionary<string, string>() { { currentBlock.ProfileValueKey, currentBlock.ProfileValueValue } };
                }

                _profileStoreService.EditOrCreateProfile("default", model);
            }

            if (currentBlock.CreateSegmentAutomatically)
            {
                var segments = _profileStoreService.GetAllSegments();

                var s = new SegmentModel
                {
                    Name = ((IContent)currentBlock).Name,
                    Scope = "default",
                    ProfileQuery = $"Payload.{currentBlock.ProfileValueKey} eq '{currentBlock.ProfileValueValue}'",
                    AvailableForPersonalization = true,
                    SegmentManager = null,
                    Description = null
                };

                var existing = segments.SegmentList.FirstOrDefault(x => x.Name.IndexOf(((IContent)currentBlock).Name, StringComparison.CurrentCultureIgnoreCase) >= 0);
                if (existing != null)
                {
                    s.SegmentId = existing.SegmentId;
                    s.Name = existing.Name;
                }
                _profileStoreService.EditOrCreateSegment(s);
            }

            var trackingData = new TrackingData<PageVisitEvent>()
            {
                EventTime = DateTime.Now,
                EventType = currentBlock.ProfileValueKey,
                User = new UserData
                {
                    Email = currentUser.Email,
                    Name = currentUser.Email
                },
                Value = currentBlock.ProfileValueValue,
                Payload = new PageVisitEvent()
            };

            _trackingService.Track(trackingData, HttpContext);

            return PartialView(currentBlock);
        }

        private UserData GetUserData()
        {
            try
            {
                var current = EPiServerProfile.Current;
                var email = new MailAddress(current.UserName);
                if (!string.IsNullOrEmpty(current.Email) || email.Address == current.UserName)
                {
                    var data = new UserData
                    {
                        Name = (current != null) ? current.UserName : "",
                        Email = (current != null) ? email.Address == current.UserName ? current.UserName : current.Email : ""
                    };
                    return data;
                }

                return null;
            }
            catch
            {
                return new UserData()
                {
                    Email = EPiServerProfile.Current.Email,
                    Name = PrincipalInfo.CurrentPrincipal.Identity.Name
                };
            }
        }
    }
}
