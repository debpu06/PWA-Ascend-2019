using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore;
using EPiServer.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Controllers
{
    [TemplateDescriptor(Default = true, ModelType = typeof(UsersInSegmentBlock))]
    public class UsersInSegmentBlockController : BlockController<UsersInSegmentBlock>
    {
        private IContentLoader contentLoader;
        private ProfileStoreServiceSync profileStoreService;

        public UsersInSegmentBlockController(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
            profileStoreService = new ProfileStoreServiceSync();
        }

        [HttpGet]
        public override ActionResult Index(UsersInSegmentBlock currentBlock)
        {
            var model = new UsersInSegmentViewModel();
            model.CurrentBlock = currentBlock;

            var segments = GetSegments(currentBlock);

            if (segments == null || segments.Count <= 0)
                return PartialView(model);

            // get profilequery
            string profileQuery = segments.SegmentList.FirstOrDefault()?.ProfileQuery;
            if (string.IsNullOrEmpty(profileQuery))
                return PartialView(model);

            // get profiles
            string filter = $"?$filter={profileQuery}";
            var profiles = profileStoreService.GetProfiles(filter);
            if (profiles == null || profiles.Count <= 0)
                return PartialView(model);

            var users = new List<UserModel>();
            // return viewmodel
            foreach (var profile in profiles.ProfileStoreList)
            {
                if (profile.Info.Email.Contains("admin")) continue;
                users.Add(new UserModel
                {
                    Email = profile.Info.Email,
                    Matched = true,
                    IsFromInsight = true
                });
            }

            // get remaining users
            var start = contentLoader.Get<StartPage>(ContentReference.StartPage);
            if (start?.QuickAccessLogins?.Any() == true)
            {
                foreach (var ql in start.QuickAccessLogins)
                {
                    if (!users.Any(x => x.Email == ql.UserName))
                    {
                        users.Add(new UserModel()
                        {
                            Email = ql.UserName,
                            Matched = false,
                            IsFromInsight = false
                        });
                    }
                }
            }

            model.UsersList = users.OrderBy(x => x.Matched).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Index(UsersInSegmentViewModel model)
        {
            // check which need updated:
            var toUpdate = model.UsersList.Where(x => x.IsFromInsight == false && x.Matched == true);
            var segments = GetSegments(model.CurrentBlock);
            var segment = segments?.SegmentList?.FirstOrDefault();
            if (segment == null)
                return PartialView("UpdateViewed");

            foreach (var profileData in toUpdate)
            {
                // get profile
                var profile = new ProfileStoreModel();
                var matches = profileStoreService.GetProfiles($"?$filter=Info.Email eq '{profileData.Email}' and Scope eq 'default'");
                if (matches.Count > 0)
                    profile = matches.ProfileStoreList.FirstOrDefault();
                else
                {
                    profile = new ProfileStoreModel
                    {
                        LastSeen = DateTime.Now,
                        Info = new ProfileStoreInformation
                        {
                            Email = profileData.Email
                        },
                        Name = profileData.Email
                    };
                }

                string query = segment.ProfileQuery;
                query = query.Replace("Payload.", "").Replace(" eq ", ",").Replace("'", "");
                var keyval = query.Split(',');
                if (profile.Payload == null)
                {
                    profile.Payload = new Dictionary<string, string>();
                }
                if (profile.Payload.ContainsKey(keyval[0]))
                {
                    profile.Payload[keyval[0]] = keyval[1];
                }
                else
                {
                    profile.Payload.Add(keyval[0], keyval[1]);
                }

                // add profile payload
                profileStoreService.EditOrCreateProfile("default", profile);
            }

            return PartialView("UpdateViewed");
        }


        private SegmentItems GetSegments(UsersInSegmentBlock currentBlock)
        {
            // get segment
            var segments = new SegmentItems();
            if (!string.IsNullOrEmpty(currentBlock?.SegmentId))
            {
                // get segment by id
                Guid segmentGuid = Guid.Empty;
                if (Guid.TryParse(currentBlock.SegmentId, out segmentGuid))
                    segments = profileStoreService.GetSegmentById(segmentGuid);
            }
            else if (currentBlock.SegmentTrackingBlock != null && currentBlock.SegmentTrackingBlock != ContentReference.EmptyReference)
            {
                // get segment by tracking block (name)
                var trackingBlock = contentLoader.Get<InsightPayloadBlock>(currentBlock.SegmentTrackingBlock);
                if (trackingBlock == null || !trackingBlock.CreateSegmentAutomatically)
                {
                    return null;
                }
                var segmentQuery = $"?$filter=Archived eq false and Name eq '{((IContent)trackingBlock).Name}'";
                segments = profileStoreService.GetSegments(segmentQuery);
            }
            else if (!string.IsNullOrEmpty(currentBlock.SegmentName))
            {
                // get segment by name
                var segmentQuery = $"?$filter=Archived eq false and Name eq '{currentBlock.SegmentName}'";
                segments = profileStoreService.GetSegments(segmentQuery);
            }

            return segments;
        }
    }
}
