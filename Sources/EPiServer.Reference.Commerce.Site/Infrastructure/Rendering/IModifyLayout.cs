using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Rendering
{
    /// <summary>
    /// Defines a method which may be invoked by PageContextActionFilter allowing controllers
    /// to modify common layout properties of the view model.
    /// </summary>
    interface IModifyLayout
    {
        void ModifyLayout(LayoutModel layoutModel);
    }
}
