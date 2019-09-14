using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    /// <summary>
    /// Used primarily for publishing news articles on the website
    /// </summary>
    [SiteContentType(DisplayName = "Two Column Left Menu Article Page",
        Description = "This page is a two column layout with a left menu that is the children of the current page.",
        GroupName = SiteTabs.Content,
        GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4")]
    [SiteImageUrl(Constants.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
    public class AlloyArticlePage : StandardPage
    {
        
    }
}
