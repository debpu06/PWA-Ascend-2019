using EPiServer.Core;
using EPiServer.Find.Cms;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.ServiceLocation;
using EPiServer.Social.ActivityStreams.Core;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Handlers
{
    /// <summary>
    /// Class ContentRatedActivityHandler.
    /// </summary>
    /// <seealso cref="EPiServer.Social.ActivityStreams.Core.IActivityHandler{SocialRatingActivity}" />
    public class ContentRatedActivityHandler : IActivityHandler<ReviewActivity>
    {
        /// <summary>
        /// Handle the specified activity.
        /// </summary>
        /// <param name="activity">The activity to be handled</param>
        /// <param name="extension">The payload of the activity being handled</param>
        public void Handle(Activity activity, ReviewActivity extension)
        {
            var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();

            var code = extension.Product;
            var productLink = referenceConverter.GetContentLink(code);

            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var product = repo.Get<BaseProduct>(productLink);

            //IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            //IContent content = contentRepository.Get<IContent>(Guid.Parse(activity.Target.Id));

            HandleFindIndexing(product);
        }

        /// <summary>
        /// Handles the find indexing.
        /// </summary>
        /// <param name="content">The content.</param>
        private static void HandleFindIndexing(IContent content)
        {
            bool shouldIndex;
            bool ok = ContentIndexer.Instance.TryShouldIndex(content, out shouldIndex);

            if (ok && shouldIndex)
            {
                ContentIndexer.Instance.Index(content);
            }
        }
    }
}