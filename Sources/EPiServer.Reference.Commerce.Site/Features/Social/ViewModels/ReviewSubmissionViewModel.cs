using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Social.ViewModels
{
    /// <summary>
    /// The ReviewSubmissionViewModel class defines a data structure
    /// supporting the contribution of reviews through the application's
    /// user interface.
    /// </summary>
    public class ReviewSubmissionViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReviewSubmissionViewModel()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productCode">Code identifying the product being reviewed</param>
        public ReviewSubmissionViewModel(string productCode)
        {
            this.ProductCode = productCode;
        }

        /// <summary>
        /// Gets or sets the unique code identifying the product under review.
        /// </summary>
        [Required]
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the title of the review.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a title for your review.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the primary message of the review.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please add a description to your review.")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the review's contributor.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide your nickname.")]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the location of the review's contributor.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide your location.")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the review contributor's rating of the associated product.
        /// </summary>
        [Range(1, 5, ErrorMessage = "Please provide a rating from 1 to 5.")]
        public int Rating { get; set; }
    }
}