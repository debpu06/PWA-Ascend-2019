using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.ServiceLocation;
using EPiServer.Social.ActivityStreams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Services

{
    [ServiceConfiguration(typeof(IReviewActivityService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ReviewActivityService : IReviewActivityService
        {
            private readonly IActivityService _activityService;

            public ReviewActivityService(IActivityService activityService)
            {
            _activityService = activityService;
            }

            public void Add(string actor, string target, ReviewActivity activity)
            {
            // Instantiate a reference for the contributor
            var contributor = EPiServer.Social.Common.Reference.Create($"visitor://{actor}");

            // Instantiate a reference for the product
            var product = EPiServer.Social.Common.Reference.Create($"product://{target}");

            _activityService.Add(new Activity(contributor, product), activity);
            }
        }
}

