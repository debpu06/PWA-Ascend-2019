using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.OrderDetails.Pages
{
    [ContentType(DisplayName = "Order details page", GUID = "11ad9718-fc02-45d0-9b98-349da9493dce", Description = "", GroupName ="Commerce", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/cms-icon-page-15.png")]
    public class OrderDetailsPage : SitePageData
    {
        
    }
}