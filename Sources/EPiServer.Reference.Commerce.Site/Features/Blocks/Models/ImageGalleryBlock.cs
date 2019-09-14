using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Image Gallery Block", GUID = "2035809e-8797-4758-93e7-d9dade59bb76", Description = "")]
    [Infrastructure.Attributes.SiteImageUrl]
    public class ImageGalleryBlock : SiteBlockData
    {
        [Display(
             Name = "Images",
             Description = "This will link to the media folder with the images for the gallery",
             GroupName = SystemTabNames.Content,
             Order = 1)]
        [Required(AllowEmptyStrings = false)]
        [UIHint(UIHint.AssetsFolder)]
        [CultureSpecific]
        public virtual ContentReference Images { get; set; }
    }
}