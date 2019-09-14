namespace EPiServer.Reference.Commerce.Site.Features.Social.Composites
{
    /// <summary>
    /// The Review class defines a data structure used by the application
    /// to compose a "review" by extending the Episerver Social Comment.
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Gets or sets the title of the review.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the review contributor.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the location of the review constributor.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the data pertaining to the rating associated
        /// with this review.
        /// </summary>
        public ReviewRating Rating { get; set; }
    }
}
