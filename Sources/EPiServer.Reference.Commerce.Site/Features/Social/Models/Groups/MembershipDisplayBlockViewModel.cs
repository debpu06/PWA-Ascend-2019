using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Social.Blocks.Groups;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.Groups
{
    /// <summary>
    /// The MembershipDisplayBlockViewModel class represents the model that will be used to
    /// feed data to the Membership Display block view.
    /// </summary>
    public class MembershipDisplayBlockViewModel
    {
        public MembershipDisplayBlockViewModel(MembershipDisplayBlock currentBlock)
        {
            Heading = currentBlock.Heading;
            ShowHeading = currentBlock.ShowHeading;
            GroupName = currentBlock.GroupName;
            Messages = new List<MessageViewModel>();
            Members = new List<CommunityMemberViewModel>();
        }
        /// <summary>
        /// Gets or sets the heading for the membership display block.
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Gets or sets whether to show the block heading .
        /// </summary>
        public bool ShowHeading { get; set; }

        /// <summary>
        /// Members displayed in the view will be associated with the group name provided in the admin view.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// List of the users associated with the group
        /// </summary>
        public List<CommunityMemberViewModel> Members { get; set; }

        /// <summary>
        /// Gets and sets message details to be displayed to the user
        /// </summary>
        public List<MessageViewModel> Messages { get; set; }
    }
}

