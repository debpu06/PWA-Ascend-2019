using EPiServer.Commerce;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.MappableImage.Model
{
    public class MappableCoordinate
    {
        [UIHint(UIHint.ProductVariation)]
        [AllowedTypes(AllowedTypes = new[] { typeof(ProductContent) })]
        [Display(Name = "Product")]
        public ContentReference CommerceContentReference { get; set; }

        [Display(Name = "Top", Description = "Use % for responsive")]
        public string TopPosition { get; set; }

        [Display(Name = "Left", Description = "Use % for responsive")]
        public string LeftPosition { get; set; }

        [Display(Name = "Link Class")]
        public string LinkClass { get; set; }

        [Display(Name = "Link Color")]
        public string LinkColor { get; set; }
    }
}