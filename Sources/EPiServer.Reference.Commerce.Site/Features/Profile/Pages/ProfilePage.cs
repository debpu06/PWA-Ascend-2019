using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Social.Blocks.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Blocks.Groups;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Pages
{
    [ContentType(DisplayName = "Profile page", GUID = "c8d44748-62e6-4121-9bdb-f5574263f007", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/elected.png")]
    public class ProfilePage : SitePageData
    {
        /// <summary>
        /// The feed section of the profile page. Local feed block will display feed items for the pages a user has subscriped to.
        /// </summary>
        [Display(
            Name = "Feed Block",
            Description = "The feed section of the profile page. Local feed block will display feed items for the pages a user has subscriped to.",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual FeedBlock Feed { get; set; }

        /// <summary>
        /// The membership affiliation section of the profile page. Local membership affiliation block will display the groups that the currently logged in user is a member of.
        /// </summary>
        [Display(
            Name = "Membership Affiliation Block",
            Description = "The membership affiliation section of the profile page. Local membership affiliation block will display the groups that the currently logged in user is a member of.",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual MembershipAffiliationBlock MembershipAffiliation { get; set; }
    }
}