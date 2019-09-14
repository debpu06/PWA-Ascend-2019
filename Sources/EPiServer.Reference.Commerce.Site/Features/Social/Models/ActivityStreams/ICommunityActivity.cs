using EPiServer.Reference.Commerce.Site.Features.Social.Adapters.ActivityStreams;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams
{
    /// <summary>
    /// This interface represents a set of operations supported by all types of community activities 
    /// with in the site.
    /// </summary>
    public interface ICommunityActivity
    {
        /// <summary>
        /// Accepts an ICommunityActivityAdapter instance capable of interpreting an activity instance.
        /// </summary>
        /// <param name="adapter">an instance of ICommunityActivityAdapter</param>
        void Accept(ICommunityActivityAdapter adapter);
    }
}