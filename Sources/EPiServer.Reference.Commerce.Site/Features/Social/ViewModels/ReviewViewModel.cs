using EPiServer.Social.Comments.Core;
using System;
using EPiServer.Social.Common;

namespace EPiServer.Reference.Commerce.Site.Features.Social.ViewModels
{
    /// <summary>
    /// The ReviewViewModel class defines a data structure supporting the 
    /// presentation of an individual product review.
    /// </summary>
    public class ReviewViewModel
    {

        public CommentId Id { get; set; }

        public virtual bool IsVisible { get; set; }

        public EPiServer.Social.Common.Reference Parent { get; set; }

        public EPiServer.Social.Common.Reference Author { get; set; }

        /// <summary>
        /// Gets or sets the title of the review.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the primary message of the review.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the review's contributor.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the location of the review's contributor.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the review contributor's rating of 
        /// the associated product.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the date on which the review was
        /// contributed.
        /// </summary>
        public DateTime AddedOn { get; set; }

        public string AddedOnStr { get; set; }
    }
}