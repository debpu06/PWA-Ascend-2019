using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels
{
    public class ContentViewModel<T> : IContentViewModel<T> where T : IContent
    {
        private Injected<IContentLoader> _contentLoader = default(Injected<IContentLoader>);
        private BaseStartPage _startPage;

        public ContentViewModel()
        {

        }

        public ContentViewModel(T currentContent)
        {
            CurrentContent = currentContent;
        }

        public T CurrentContent { get; set; }

        public virtual BaseStartPage StartPage => _startPage ?? (_startPage = _contentLoader.Service.Get<BaseStartPage>(ContentReference.StartPage));
        public LayoutModel Layout { get; set; }
        public IContent Section { get; set; }
    }

    public static class ContentViewModel
    {
        /// <summary>
        /// Returns a PageViewModel of type <typeparam name="T"/>.
        /// </summary>
        /// <remarks>
        /// Convenience method for creating PageViewModels without having to specify the type as methods can use type inference while constructors cannot.
        /// </remarks>
        public static ContentViewModel<T> Create<T>(T page) where T : IContent
        {
            return new ContentViewModel<T>(page);
        }
    }
}