using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Pages
{
    [ContentType(DisplayName = "Shared Cart Page", GUID = "701b5df0-fa41-40cb-807f-645be22714cc", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/cms-icon-page-08.png")]
    public class SharedCartPage : SitePageData
    {
        /*
                [CultureSpecific]
                [Display(
                    Name = "Main body",
                    Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual XhtmlString MainBody { get; set; }
         */
    }
}