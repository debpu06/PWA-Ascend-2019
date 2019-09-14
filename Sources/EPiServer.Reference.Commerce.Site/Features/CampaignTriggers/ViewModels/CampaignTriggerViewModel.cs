using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.ViewModels
{
    public enum TriggerId
    {
        AbandonedBasket,
        Purchased3Days,
        AbandonedBrowse,
        NoLogin,
        SignUpLoyalty,
        InactiveCustomer
    };

    public class CampaignTriggerViewModel : ContentViewModel<CampaignTriggerPage>
    {
        public CampaignTriggerViewModel(CampaignTriggerPage currentContent) : base(currentContent)
        {

        }

        [Display(Name = "Id of the recipient list which all test emails to be sent must exist in this")]
        public string RecipientListId { get; set; }

        [Display(Name = "Mailing Id")]
        public string MailingId { get; set; }

        [Display(Name = "Test email accounts")]
        public string EmailList { get; set; }
        public string Result { get; set; }
    }

    public class CampaignRecipient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsTestList { get; set; }
    }

    public class CampaignMail
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}