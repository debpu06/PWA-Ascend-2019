using EPiServer.Reference.Commerce.Shared;
using EPiServer.Shell.Security;
using EPiServer.Web.Mvc;
using System;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Infrastructure.Rendering;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    /// <summary>
    /// All controllers that renders pages should inherit from this class so that we can 
    /// apply action filters, such as for output caching site wide, should we want to.
    /// </summary>
    public abstract class PageControllerBase<T> : PageController<T>, IModifyLayout
        where T : SitePageData
    {
        protected EPiServer.ServiceLocation.Injected<UISignInManager> UISignInManager;
        /// <summary>
        /// Signs out the current user and redirects to the Index action of the same controller.
        /// </summary>
        /// <remarks>
        /// There's a log out link in the footer which should redirect the user to the same page. 
        /// As we don't have a specific user/account/login controller but rely on the login URL for 
        /// forms authentication for login functionality we add an action for logging out to all
        /// controllers inheriting from this class.
        /// </remarks>
        public ActionResult Logout()
        {
            UISignInManager.Service.SignOut();
            TrackingCookieManager.SetTrackingCookie(Guid.NewGuid().ToString());
            return RedirectToAction("Index");
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            // don't throw 404 in edit mode
            if (!EPiServer.Editor.PageEditing.PageIsInEditMode)
            {
                if (PageContext?.Page != null && PageContext.Page.StopPublish <= DateTime.Now)
                {
                    filterContext.Result = new HttpStatusCodeResult(404, "Not found");
                    return;
                }
            }

            base.OnAuthorization(filterContext);
        }

        public virtual void ModifyLayout(LayoutModel layoutModel)
        {
            var page = PageContext.Page as SitePageData;
            if(page != null)
            {
                layoutModel.HideHeader = page.HideSiteHeader;
                layoutModel.HideFooter = page.HideSiteFooter;
            }
        }
    }
}
