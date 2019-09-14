using System.Linq;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.B2B.Services;
using EPiServer.Web.Mvc;
using System.Web.Mvc;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Reference.Commerce.Shared.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace EPiServer.Reference.Commerce.Site.Features.SalesRep
{
    public class SalesRepController : PageController<SalesRepPage>
    {
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<SiteUser> _userManager;
        private readonly ApplicationSignInManager<SiteUser> _signInManager;
        private readonly CookieService _cookieService = new CookieService();

        public SalesRepController(IOrganizationService organizationService, 
            ApplicationSignInManager<SiteUser> signInManager, 
            ApplicationUserManager<SiteUser> userManager)
        {
            _organizationService = organizationService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Index(SalesRepPage currentPage)
        {
            return View(new SalesRepViewModel(currentPage)
            {
                Organizations = _organizationService.GetOrganizationModels()
            });
        }

        [HttpPost]
        public ActionResult ImpersonateUser(string username)
        {
            var success = false;
            var user = _userManager.FindByEmail(username);
            if (user != null)
            {
                _cookieService.Set(B2B.Constants.Cookies.B2BImpersonatingAdmin, User.Identity.GetUserName(), true);
                _signInManager.SignIn(user, false, false);
                success = true;
            }
            return Json(new { success });
        }

        public ActionResult BackAsAdmin()
        {
            var adminUsername = _cookieService.Get(B2B.Constants.Cookies.B2BImpersonatingAdmin);
            if (!string.IsNullOrEmpty(adminUsername))
            {
                var adminUser = _userManager.FindByEmail(adminUsername);
                if (adminUser != null)
                    _signInManager.SignIn(adminUser, false, false);

                _cookieService.Remove(B2B.Constants.Cookies.B2BImpersonatingAdmin);
            }
            return Redirect(Request.UrlReferrer?.AbsoluteUri ?? "/");
        }
    }
}