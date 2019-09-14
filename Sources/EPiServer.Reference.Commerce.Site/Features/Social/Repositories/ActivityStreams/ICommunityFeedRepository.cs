using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Repositories.ActivityStreams
{
    /// <summary>
    /// The ICommunityFeedRepository interface defines the operations for accessing feeds of community activities. 
    /// </summary>
    public interface ICommunityFeedRepository
    {
        /// <summary>
        ///  Retrieves feed items based on the specified filter.
        /// </summary>
        /// <param name="filter">A feed item filter</param>
        /// <returns>A list of feed items.</returns>
        IEnumerable<CommunityFeedItemViewModel> Get(CommunityFeedFilter filter);
    }
}
