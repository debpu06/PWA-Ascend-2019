using EPiServer.Personalization;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class AlloyProfilePageController : PageControllerBase<AlloyProfilePage>
    {
        [HttpPost]
        [ValidateInput(false)]
        public ViewResult Save(AlloyProfilePage currentPage, AlloyProfilePageViewModel model2)
        {
            model2.Profile.Save();

            var model = new AlloyProfilePageViewModel(currentPage)
            {
                Profile = EPiServerProfile.Current,
                //Impersonating = false
            };
            
            return View("~/Features/Alloy/Views/AlloyProfilePage/Index.cshtml", model);
        }

        public ViewResult Save(AlloyProfilePage currentPage)
        {

            //model2.Profile.Save();

            var model = new AlloyProfilePageViewModel(currentPage)
            {
                Profile = EPiServerProfile.Current,
                //Impersonating = false
            };
            return View("~/Features/Alloy/Views/AlloyProfilePage/Index.cshtml", model);
        }

        public ViewResult Index(AlloyProfilePage currentPage)
        {

            var model = new AlloyProfilePageViewModel(currentPage)
            {
                Profile = EPiServerProfile.Current,
                //Impersonating = false
            };
            
            return View(model);
        }

        //public ViewResult Impersonate(ProfilePage currentPage, string users)
        //{
        //    EPiServerProfile ImpersonatedUser = EPiServerProfile.Get(users);


        //    var model = new ProfilePageViewModel(currentPage)
        //    {
        //        Profile = ImpersonatedUser,
        //        //Impersonating = true
        //    };

        //    List<string> users2 = new List<string>();

        //    users2.Add("Jacob");
        //    users2.Add("Rik");
        //    users2.Add("Dean");

        //    ViewData["users"] = new SelectList(users2);

        //    return View("~/Views/ProfilePage/Index.cshtml", model);
        //}

    }
}

