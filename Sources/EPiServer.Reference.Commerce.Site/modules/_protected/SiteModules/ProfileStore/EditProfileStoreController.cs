using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Shell.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class EditProfileStoreController : Controller
    {
        private readonly IProfileStoreService _profileStoreService;

        public ProfileStoreModel ProfileStoreData { get; private set; }

        public EditProfileStoreController(IProfileStoreService profileStoreService)
        {
            _profileStoreService = profileStoreService;
        }

        /// <summary>
        /// Shows dashboard
        /// </summary>
        /// <returns></returns>
        [MenuItem("/global/profiles/dashboard", Text = "Dashboard")]
        public ActionResult Index()
        {
            var profileStoreFilterOptions = new ProfileStoreFilterOptions()
            {
                Skip = 0,
                Top = 100
            };

            var profileStoreItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllProfiles(profileStoreFilterOptions));
            var scopeItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllScopes());
            var segmentItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllSegments());
            var blacklistItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllBlacklist());

            var viewModel = new EditProfileStoreViewModel()
            {
                ProfileStoreItems = profileStoreItems,
                ScopeItems = scopeItems,
                SegmentItems = segmentItems,
                BlacklistItems = blacklistItems
            };

            return View(viewModel);
        }

        /// <summary>
        /// Lists all profile store records
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <returns></returns>
        public ActionResult ProfileStoreDetail(PagingInfo pagingInfo)
        {
            var viewModel = new EditProfileStoreViewModel();
            var pageSize = pagingInfo.PageSize == 0 ? 20 : pagingInfo.PageSize;
            var skip = pageSize * ((pagingInfo.PageNumber == 0 ? 1 : pagingInfo.PageNumber) - 1);

            var profileFilterStoreOptions = new ProfileStoreFilterOptions()
            {
                Skip = skip,
                Top = pageSize
            };

            var profileStoreItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllProfiles(profileFilterStoreOptions));

            if (profileStoreItems.Total != 0)
            {
                viewModel = new EditProfileStoreViewModel()
                {
                    ProfileStoreItems = profileStoreItems,
                    PagingInfo = pagingInfo
                };
                viewModel.PagingInfo.PageSize = pageSize;
                viewModel.PagingInfo.TotalRecord = profileStoreItems.Total;
                viewModel.PagingInfo.PageNumber = pagingInfo.PageNumber == 0 ? 1 : pagingInfo.PageNumber;
            }
            else
            {
                viewModel = new EditProfileStoreViewModel()
                {
                    ProfileStoreItems = null
                };
            }

            return View(viewModel);
        }

        /// <summary>
        /// Shows visitor information
        /// </summary>
        /// <returns></returns>
        [MenuItem("/global/profiles/visitors", Text = "Visitors")]
        public ActionResult Visitors()
        {
            var profileStoreItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllProfiles(null));
            var scopeItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllScopes());
            var segmentItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllSegments());
            var blacklistItems = AsyncHelpers.RunSync(() => _profileStoreService.GetAllBlacklist());

            var viewModel = new EditProfileStoreViewModel()
            {
                ProfileStoreItems = profileStoreItems,
                ScopeItems = scopeItems,
                SegmentItems = segmentItems,
                BlacklistItems = blacklistItems
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditProfileForm(string scope, string profileId)
        {
            var model = AsyncHelpers.RunSync(() => _profileStoreService.GetProfileById(scope, new System.Guid(profileId)));
            EditProfileStoreViewModel viewModel = new EditProfileStoreViewModel()
            {
                ProfileStoreModel = model
            };
            _profileStoreService.LoadCountry(viewModel.ProfileStoreModel);
            return View("EditProfileForm", viewModel);
        }

        /// <summary>
        /// Save profile
        /// </summary>
        /// <param name="viewModel">Data model of Profile Store</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveProfile(EditProfileStoreViewModel viewModel)
        {
            AsyncHelpers.RunSync(() => _profileStoreService.EditOrCreateProfile(viewModel.ProfileStoreModel.Scope, viewModel.ProfileStoreModel));
            return RedirectToAction("ProfileStoreDetail");
        }

        [HttpGet]
        public ActionResult GetPageViewEvents()
        {
            var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";
            DateTime[] last7Days = Enumerable.Range(0, 7)
                                    .Select(i => DateTime.Now.Date.AddDays(-i))
                                    .ToArray();
            var result = new List<VisualizationModel>();
            foreach (var day in last7Days)
            {
                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = AsyncHelpers.RunSync(() => _profileStoreService.GetVisualizationItems(queryString));
                if (items != null)
                {
                    result.AddRange(items.VisualizationModels);
                }
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public ActionResult GetSearchEvents()
        {
            var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiSearch and EventTime eq {0}";
            DateTime[] last7Days = Enumerable.Range(0, 7)
                                    .Select(i => DateTime.Now.Date.AddDays(-i))
                                    .ToArray();
            var result = new List<VisualizationModel>();
            foreach (var day in last7Days)
            {
                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = AsyncHelpers.RunSync(() => _profileStoreService.GetVisualizationItems(queryString));
                if (items != null)
                {
                    result.AddRange(items.VisualizationModels);
                }
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result),
                ContentType = "application/json",
            };
        }
    }
}