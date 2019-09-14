using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams
{
    /// <summary>
    /// The SubscriptionFormViewModel class represents the model of a social subscription form view.
    /// </summary>
    public class SubscriptionFormViewModel
    {
        /// <summary>
        /// Default parameterless constructor required for view form submitting.
        /// </summary>
        public SubscriptionFormViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the reference link of the page containing the subscription form.
        /// </summary>
        public PageReference CurrentPageLink { get; set; }
    }
}