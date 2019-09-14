using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Controllers
{
    public class BlogListPageController : PageController<BlogListPage>
    {

        public ActionResult Index(BlogListPage currentPage)
        {
            var model = new ContentViewModel<BlogListPage>(currentPage);
            return View(model);
        }


    }
}
