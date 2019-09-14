using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Start.Pages
{
    [ContentType(DisplayName = "Start page", GUID = "452d1812-7385-42c3-8073-c1b7481e7b20", Description = "", AvailableInEditMode = true, GroupName ="Start")]
    [SiteImageUrl("~/content/icons/pages/CMS-icon-page-02.png")]
    public class StartPage : BaseStartPage
    {
        
    }
}