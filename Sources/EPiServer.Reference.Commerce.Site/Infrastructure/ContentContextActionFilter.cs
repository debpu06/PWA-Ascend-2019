using System.Web.Mvc;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Rendering;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    /// <summary>
    /// Intercepts actions with view models of type IPageViewModel and populates the view models
    /// Layout and Section properties.
    /// </summary>
    /// <remarks>
    /// This filter frees controllers for pages from having to care about common context needed by layouts
    /// and other page framework components allowing the controllers to focus on the specifics for the page types
    /// and actions that they handle. 
    /// </remarks>
    public class ContentContextActionFilter : IResultFilter
    {
        private readonly ContentViewContextFactory _contextFactory;
        public ContentContextActionFilter(ContentViewContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model;

            var model = viewModel as IContentViewModel<SitePageData>;
            if (model != null)
            {
                var currentContentLink = filterContext.RequestContext.GetContentLink();
                
                var layoutModel = model.Layout ?? _contextFactory.CreateLayoutModel(currentContentLink, filterContext.RequestContext);
                
                var layoutController = filterContext.Controller as IModifyLayout;
                if(layoutController != null)
                {
                    layoutController.ModifyLayout(layoutModel);
                }
                
                model.Layout = layoutModel;

                if (model.Section == null && currentContentLink != null)
                {
                    model.Section = _contextFactory.GetSection(currentContentLink);
                }

                if (!string.IsNullOrEmpty(model.Layout.MasterName) && filterContext.Result is ViewResult && !(model.CurrentContent is IUseDirectLayout))
                {
                    (filterContext.Result as ViewResult).MasterName = model.Layout.MasterName;
                }

            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
