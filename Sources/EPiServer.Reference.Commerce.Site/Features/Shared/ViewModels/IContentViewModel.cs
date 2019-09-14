using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels
{
    /// <summary>
    /// Defines common characteristics for view models for pages, including properties used by layout files.
    /// </summary>
    /// <remarks>
    /// Views which should handle several page types (T) can use this interface as model type rather than the
    /// concrete PageViewModel class, utilizing the that this interface is covariant.
    /// </remarks>
    public interface IContentViewModel<out T> where T : IContent
    {
        T CurrentContent { get; }
        BaseStartPage StartPage { get; }
        LayoutModel Layout { get; set; }
        IContent Section { get; set; }
    }
}
