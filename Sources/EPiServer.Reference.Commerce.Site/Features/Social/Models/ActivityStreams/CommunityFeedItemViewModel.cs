using System;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams
{
    /// <summary>
    /// The view model of a feed item displayed in a feed block.
    /// </summary>
    public class CommunityFeedItemViewModel
    {
        /// <summary>
        /// A header summarizing the activity that occurred.
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// A description of the activity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The date/time on which the activity was received by the Social Framework.
        /// </summary>
        public DateTime ActivityDate { get; set; }
    }
}