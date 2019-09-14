using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Rendering
{
    public abstract class SiteViewPage : SiteViewPage<object>
    {
    }

    public abstract class SiteViewPage<T> : WebViewPage<T>
    {
        protected SiteViewPage()
        {

        }

        private Injected<IContentLoader> _contentLoader;

        private BaseStartPage _startPage;
        public virtual BaseStartPage StartPage => _startPage ?? (_startPage = _contentLoader.Service.Get<BaseStartPage>(ContentReference.StartPage));
        public bool IsMosey => SiteDefinition.Current.IsMosey();
    }
}