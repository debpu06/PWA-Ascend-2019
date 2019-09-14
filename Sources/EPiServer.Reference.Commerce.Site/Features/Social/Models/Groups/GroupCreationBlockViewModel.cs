using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Social.Blocks.Groups;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.Groups
{
    /// <summary>
    /// The GroupCreationsBlockViewModel class represents the model that will be used to
    /// feed data to the GroupCreation block frontend view.
    /// </summary>
    public class GroupCreationBlockViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GroupCreationBlockViewModel()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="block">The block instance</param>
        /// <param name="currentPageLink">The current page reference</param>
        public GroupCreationBlockViewModel(GroupCreationBlock block, PageReference currentPageLink)
        {
            Heading = block.Heading;
            ShowHeading = block.ShowHeading;
            CurrentPageLink = currentPageLink;
        }

        /// <summary>
        /// Gets or sets the heading for the Group Creation block.
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Gets or sets whether to show the block heading .
        /// </summary>
        public bool ShowHeading { get; set; }

        /// <summary>
        /// Gets or sets message details to be displayed to the user
        /// </summary>
        public List<MessageViewModel> Messages { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the group description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the
        /// group is moderated.
        /// </summary>
        public bool IsModerated { get; set; }

        /// <summary>
        /// Gets or sets the reference link of the page containing the group creation form.
        /// </summary>
        public PageReference CurrentPageLink { get; set; }
    }
}

