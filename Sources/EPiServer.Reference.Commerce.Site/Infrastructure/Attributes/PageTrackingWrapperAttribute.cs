using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.ServiceLocation;
using EPiServer.Tracking.PageView;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    public class PageTrackingWrapperAttribute : PageViewTrackingAttribute
    {
        private static readonly Type PageController = typeof(PageController<>);
        private readonly Injected<IContentRouteHelper> _routeHelper = default(Injected<IContentRouteHelper>);
        private readonly Injected<IContextModeResolver> _contextModeResolver = default(Injected<IContextModeResolver>);

        private bool PageIsInViewMode() => _contextModeResolver.Service.CurrentMode == ContextMode.Default;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!PageIsInViewMode())
            {
                return;
            }

            Object currentContent;
            if ((filterContext.ActionParameters.ContainsKey("currentContent")
                || filterContext.ActionParameters.ContainsKey("currentBlock"))
                && filterContext.IsChildAction)
            {
                if (filterContext.ActionParameters.TryGetValue("currentContent", out currentContent) || filterContext.ActionParameters.TryGetValue("currentBlock", out currentContent))
                {
                    var actualBlock = currentContent as BlockData;
                    if (actualBlock != null)
                    {
                        var currentPage = _routeHelper.Service.Content;
                        actualBlock.TrackBlock(currentPage, filterContext.HttpContext);
                        return;
                    }

                    var actualImage = currentContent as ImageData;
                    if (actualImage != null)
                    {
                        var currentPage = _routeHelper.Service.Content;
                        actualImage.TrackImageView(currentPage, filterContext.HttpContext);
                    }

                }
            }
            else
            {
                // Page tracking
                if ((filterContext.ActionParameters.ContainsKey("currentContent")
                     || filterContext.ActionParameters.ContainsKey("currentPage"))
                    && !filterContext.IsChildAction
                    && IsAssignableToGenericType(filterContext.Controller.GetType(), PageController))
                {
                    // Page tracking
                    if (filterContext.ActionParameters.TryGetValue("currentContent", out currentContent) || filterContext.ActionParameters.TryGetValue("currentPage", out currentContent))
                    {
                        var pageData = currentContent as PageData;
                        if (pageData != null)
                        {
                            pageData.AddPageBrowseHistory();
                            base.OnActionExecuting(filterContext);
                            pageData.TrackVisitorGroups(filterContext.HttpContext);
                        }
                    }
                }
            }


        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            foreach (var it in givenType.GetInterfaces())
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}