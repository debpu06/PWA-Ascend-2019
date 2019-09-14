using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Login.Services;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Controllers;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Controllers
{
    [Authorize]
    public class AccountInformationController : IdentityControllerBase<AccountInformationPage>
    {
        private readonly CustomerContextFacade _customerContext;

        public AccountInformationController(CustomerContextFacade customerContextFacade,
            IAddressBookService addressBookService,
            IOrderRepository orderRepository,
            ApplicationSignInManager<SiteUser> signinManager,
            ApplicationUserManager<SiteUser> userManager,
            UserService userService) : base(signinManager, userManager, userService)
        {
            _customerContext = customerContextFacade;
        }

        public ActionResult Index(AccountInformationPage currentPage)
        {
            var user = UserService.GetUser(User.Identity.Name);
            var contact = _customerContext.CurrentContact.CurrentContact;
            var viewModel = new AccountInformationViewModel(currentPage)
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                DateOfBirth = contact.BirthDate,
                SubscribesToNewsletter = user.NewsLetter
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowDBWrite]
        public ActionResult Save(AccountInformationViewModel viewModel)
        {
            var user = UserService.GetUser(User.Identity.Name);
            var contact = _customerContext.CurrentContact.CurrentContact;
            user.FirstName = contact.FirstName = viewModel.FirstName;
            user.LastName = contact.LastName = viewModel.LastName;
            contact.BirthDate = viewModel.DateOfBirth;
            user.NewsLetter = viewModel.SubscribesToNewsletter;

            UserManager.UpdateAsync(user).
                GetAwaiter().
                GetResult();

            contact.SaveChanges();

            return RedirectToAction("Index", new { node = viewModel.CurrentContent.ContentLink });
        }
    }
}