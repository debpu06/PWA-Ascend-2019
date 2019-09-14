using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.ErrorHandling.Pages
{
    [ContentType(
        DisplayName = "Error page", 
        GUID = "E7DCAABC-05BC-4230-A2AF-94CD9A9ED535", 
        Description = "The page which allows you to show errors details.", 
        AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-09.png")]
    public class ErrorPage : SitePageData
    {
        [CultureSpecific]
        [Display(
               Name = "Title",
               Description = "Title for the page",
               GroupName = SystemTabNames.Content,
               Order = 1)]
        public virtual string Title { get; set; }

    }
}