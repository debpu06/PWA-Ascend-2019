namespace EPiServer.Reference.Commerce.Site.Features.Social.ViewModels
{
    /// <summary>
    /// The ReviewStatisticsViewModel class defines a data structure
    /// supporting the display of aggregate statistics associated
    /// with the reviews of a product.
    /// </summary>
    public class ReviewStatisticsViewModel
    {
        /// <summary>
        /// Gets or sets the average of all ratings contributed
        /// for a product.
        /// </summary>
        public double OverallRating { get; set; }

        /// <summary>
        /// Gets or sets the total number of ratings contributed
        /// for a product.
        /// </summary>
        public long TotalRatings { get; set; }

        public string Code { get; set; }
    }
}
