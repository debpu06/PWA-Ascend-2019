namespace EPiServer.Reference.Commerce.Site.Features.Social.Composites
{
    /// <summary>
    /// The ReviewRating class defines a data structure, which maintains
    /// the details of a rating associated with with a review.
    /// </summary>
    public class ReviewRating
    {
        /// <summary>
        /// Gets or sets the value of the rating which has been
        /// stored in conjunction with this review.
        /// </summary>
        /// <remarks>
        /// While this value is stored as an Episerver Social Rating,
        /// have access to the value, cached on the review, we minimize
        /// the need for multiple lookups.
        /// </remarks>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets a reference to the Episerver Social
        /// Rating which has been stored in conjunction with
        /// this review.
        /// </summary>
        /// <remarks>
        /// By maintaining a reference to the Episerver Social
        /// Rating associated with this review, we enable ourselves
        /// to implement functionality that would allow us to edit
        /// the rating later.
        /// </remarks>
        public string Reference { get; set; }
    }
}
