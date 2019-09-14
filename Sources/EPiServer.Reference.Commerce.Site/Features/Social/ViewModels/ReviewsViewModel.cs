using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Social.ViewModels
{
    /// <summary>
    /// The ReviewsViewModel class defines a data structure supporting the 
    /// presentation of a collection of product reviews and their 
    /// accompanying statistical information.
    /// </summary>
    public class ReviewsViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReviewsViewModel()
        {
            this.Reviews = new List<ReviewViewModel>();
            this.Statistics = new ReviewStatisticsViewModel();
        }

        /// <summary>
        /// Gets or sets the statistical data associated with the
        /// collection of product reviews.
        /// </summary>
        public ReviewStatisticsViewModel Statistics { get; set; }        

        /// <summary>
        /// Gets or sets a collection of product reviews.
        /// </summary>
        public IEnumerable<ReviewViewModel> Reviews { get; set; }
    }
}