using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Shared.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels
{
    public class AccountInformationViewModel : ContentViewModel<AccountInformationPage>
    {
        public AccountInformationViewModel()
        {
            
        }
        public AccountInformationViewModel(AccountInformationPage accountInformationPage) : base(accountInformationPage)
        {
            
        }

        [LocalizedDisplay("/Shared/Address/Form/Label/FirstName")]
        [LocalizedRequired("/Shared/Address/Form/Empty/FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplay("/Shared/Address/Form/Label/LastName")]
        [LocalizedRequired("/Shared/Address/Form/Empty/LastName")]
        public string LastName { get; set; }

        [LocalizedDisplay("/AccountInformation/Form/DateOfBirth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [LocalizedDisplay("/AccountInformation/Form/SubscribesToNewsletter")]
        public bool SubscribesToNewsletter { get; set; }
    }
}