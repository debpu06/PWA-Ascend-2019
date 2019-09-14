using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class AlloyStartPageController : PageControllerBase<AlloyStartPage>
    {
        public ActionResult Index(AlloyStartPage currentPage)
        {
            var model = ContentViewModel.Create(currentPage);

            if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink)) // Check if it is the StartPage or just a page of the StartPage type.
            {
                //Connect the view models logotype property to the start page's to make it editable
                var editHints = ViewData.GetEditHints<ContentViewModel<AlloyStartPage>, AlloyStartPage>();
                editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
                editHints.AddConnection(m => m.Layout.ProductPages, p => p.ProductPageLinks);
                editHints.AddConnection(m => m.Layout.CompanyInformationPages, p => p.CompanyInformationPageLinks);
                editHints.AddConnection(m => m.Layout.NewsPages, p => p.NewsPageLinks);
                editHints.AddConnection(m => m.Layout.CustomerZonePages, p => p.CustomerZonePageLinks);
            }

            return View(model);
        }

    }
}
