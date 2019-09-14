using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.ViewModels;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Controllers
{
    public class TrackingUserPageController : PageController<TrackingUserPage>
    {
        private readonly ApplicationUserManager<SiteUser> _userManager;
        private readonly IContentRepository _contentRepository;
        private readonly IProfileStoreService _profileStoreService;

        public TrackingUserPageController(ApplicationUserManager<SiteUser> userManager, 
                                          IContentRepository contentRepository,
                                          IProfileStoreService profileStoreService)
        {
            _userManager = userManager;
            _contentRepository = contentRepository;
            _profileStoreService = profileStoreService;
        }

        public ActionResult Index(TrackingUserPage currentPage)
        {
            TrackingUserPageViewModel viewModel = new TrackingUserPageViewModel()
            {
                TrackingPageUrl = currentPage.TrackingPageLink == null ? null : UrlResolver.Current.GetUrl(currentPage.TrackingPageLink).ToString(),
                IsUserAdmin = EPiServer.Security.PrincipalInfo.HasAdminAccess
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SetupUser(string userName, string firstName, string lastName, string description)
        {
            bool userCreationSuccess = await CreateUser(userName, firstName, lastName);
            string deviceId = await AddUserToProfileStore(firstName, lastName, userName);
            AddUserToDropDown(userName, firstName, lastName, description, deviceId);

            return RedirectToAction("Index");
        }

        private async Task<string> AddUserToProfileStore(string firstName, string lastName, string userName)
        {
            string deviceId = Guid.NewGuid().ToString();
            ProfileStoreModel newUser = new ProfileStoreModel()
            {
                Name = string.Format("{0} {1}", firstName, lastName),
                ProfileManager = string.Empty,
                FirstSeen = DateTime.Now,
                LastSeen = DateTime.Now,
                Visits = 10,
                Info = new ProfileStoreInformation()
                {
                    Email = userName
                },
                ContactInformation = new List<string>(),
                DeviceIds = new List<object> { deviceId }
            };

            await _profileStoreService.EditOrCreateProfile("", newUser);

            return deviceId;
        }

        private void AddUserToDropDown(string userName, string firstName, string lastName, string description, string deviceId)
        {
            //iterate through start page users in dropdown, add user if they don't already exist
            bool exists = false;
            StartPage start = _contentRepository.Get<StartPage>(ContentReference.StartPage);
            System.Collections.Generic.IList<SwitchableUser> users = start.QuickAccessLogins;

            if (users != null)
            {
                foreach (SwitchableUser user in users)
                {
                    if (user.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                SwitchableUser newUser = new SwitchableUser()
                {
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    UserDescription = description,
                    UserTrackingCode = deviceId
                };

                var editableStartPage = start.CreateWritableClone() as StartPage;

                if (editableStartPage.QuickAccessLogins == null)
                {
                    editableStartPage.QuickAccessLogins = new List<SwitchableUser>();
                }
                editableStartPage.QuickAccessLogins.Add(newUser);

                _contentRepository.Publish((IContent)editableStartPage);
            }
        }

        private async Task<bool> CreateUser(string userName, string firstName, string lastName)
        {
            IdentityResult result = null;
            CustomerContact contact = null;

            SiteUser existingUser = _userManager.FindByEmail(userName);

            if (existingUser == null)
            {
                var user = new SiteUser
                {
                    UserName = userName,
                    Email = userName,
                    Password = "Episerver1!",
                    FirstName = firstName,
                    LastName = lastName,
                    RegistrationSource = "Tracking Page",
                    NewsLetter = false,
                    Addresses = null,
                    IsApproved = true
                };

                result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    contact = CreateCustomerContact(user);
                    return true;
                }

                return false;
            }

            return true;
        }

        private CustomerContact CreateCustomerContact(SiteUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            CustomerContact contact = CustomerContext.Current.GetContactByUserId(string.Format("String:{0}", user.UserName));

            if (contact == null)
            {
                contact = CustomerContact.CreateInstance();

                if (!String.IsNullOrEmpty(user.FirstName) || !String.IsNullOrEmpty(user.LastName))
                {
                    contact.FullName = String.Format("{0} {1}", user.FirstName, user.LastName);
                }

                contact.PrimaryKeyId = new PrimaryKeyId(new Guid(user.Id));
                contact.FirstName = user.FirstName;
                contact.LastName = user.LastName;
                contact.Email = user.Email;
                contact.UserId = "String:" + user.Email; // The UserId needs to be set in the format "String:{email}". Else a duplicate CustomerContact will be created later on.
                contact.RegistrationSource = user.RegistrationSource;

                if (user.Addresses != null)
                {
                    foreach (var address in user.Addresses)
                    {
                        contact.AddContactAddress(address);
                    }
                }

                contact.SaveChanges();
            }

            return contact;
        }

    }
}