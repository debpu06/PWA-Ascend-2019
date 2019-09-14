using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.B2B.Services;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Shell.Navigation;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Users
{
    public class ManageUsersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IOrganizationService _organizationService;
        private readonly IContentLoader _contentLoader;
        private readonly IMailService _mailService;
        private readonly ApplicationUserManager<SiteUser> _userManager;
        private readonly ApplicationSignInManager<SiteUser> _signInManager;
        private readonly LocalizationService _localizationService;
        private readonly IEPiFindSearchService _ePiFindSearchService;
        private readonly CookieService _cookieService;

        public ManageUsersController(ICustomerService customerService, IOrganizationService organizationService,
            ApplicationUserManager<SiteUser> userManager,
            ApplicationSignInManager<SiteUser> signinManager,
            IContentLoader contentLoader, IMailService mailService, LocalizationService localizationService,
            IEPiFindSearchService ePiFindSearchService, CookieService cookieService)
        {
            _customerService = customerService;
            _organizationService = organizationService;
            _userManager = userManager;
            _signInManager = signinManager;
            _contentLoader = contentLoader;
            _mailService = mailService;
            _localizationService = localizationService;
            _ePiFindSearchService = ePiFindSearchService;
            _cookieService = cookieService;
        }

        [MenuItem("/global/cms/manageusers", Text = "Impersonate")]
        public ActionResult Index()
        {
            var data = _ePiFindSearchService.SearchUsers("");
            return View(data);
        }

        //private ContactViewModel GetModel(UserSearchResultModel contact)
        //{
        //    return new ContactViewModel
        //    {
        //        ContactId = contact.ContactId,
        //        FirstName = contact.FirstName,
        //        LastName = contact.LastName,
        //        Email = contact.Email,
        //        Organization = contact.B2BOrganization != null ? new OrganizationModel(contact.B2BOrganization) : null,
        //        UserRole = contact.UserRole,
        //        Budget = contact.Budget != null ? new BudgetViewModel(contact.Budget) : null;
        //        Location = contact.UserLocationId;
        //    }
        //}
    }
}