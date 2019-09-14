using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.ResetPassword.Pages
{
    [ContentType(DisplayName = "Reset password page", GUID = "05834347-8f4f-48ec-a74c-c46278654a92", 
        Description = "Page for allowing users to reset their passwords. The page must also be set in the StartPage's ResetPasswordPage property."
        , AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-09.png")]
    public class ResetPasswordPage : SitePageData
    {
    }
}