using System.Collections.Generic;
using EPiServer.Commerce;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media;

namespace EPiServer.Reference.Commerce.Site.Features.Media.Models
{
    [ContentType(GUID = "20644be7-3ca1-4f84-b893-ee021b73ce6c")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageMediaData : CommerceImage, IFileProperties
    {
        [CultureSpecific]
        [Display(
            Name= "Alternate text",
            Description= "Description of the image",
            GroupName = SystemTabNames.Content,
            Order = 50)]
        public virtual string Description { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Link",
            Description = "Link to content",
            GroupName = SystemTabNames.Content,
            Order = 60)]
        [UIHint(UIHint.AllContent)]
        public virtual ContentReference Link { get; set; }

        [Display(
            Order = 10)]
        public virtual string Title { get; set; }

        [Display(
            Order = 20)]
        public virtual string AltText { get; set; }

        public virtual string Copyright { get; set; }

        [Editable(false)]
        public virtual string FileSize { get; set; }

        [Display(
            Order = 30)]
        public virtual string CreditsText { get; set; }

        [Display(
            Order = 40)]
        public virtual Url CreditsLink { get; set; }

        //Tiny 75x40
        [ScaffoldColumn(false)]
        [ImageScaleDescriptor(Width = 120, Height = 64, ScaleMethod = ImageScaleType.ScaleToFill)]
        public virtual Blob Tiny { get; set; }

        //Small 150x80
        [ScaffoldColumn(false)]
        [ImageScaleDescriptor(Width = 150, Height = 80, ScaleMethod = ImageScaleType.ScaleToFill)]
        public virtual Blob Small { get; set; }

        //Medium 300x160
        [ScaffoldColumn(false)]
        [ImageScaleDescriptor(Width = 375, Height = 200, ScaleMethod = ImageScaleType.ScaleToFill)]
        public virtual Blob Medium { get; set; }

        //Large 750x400
        [ScaffoldColumn(false)]
        [ImageScaleDescriptor(Width = 900, Height = 480, ScaleMethod = ImageScaleType.ScaleToFill)]
        public virtual Blob Large { get; set; }

        //XLarge 1500x800
        [ScaffoldColumn(false)]
        [ImageScaleDescriptor(Width = 1200, Height = 640, ScaleMethod = ImageScaleType.ScaleToFill)]
        public virtual Blob XLarge { get; set; }

        public virtual string AccentColor { get; set; }

        public virtual string Caption { get; set; }

        public virtual string ClipArtType { get; set; }

        public virtual string DominantColorBackground { get; set; }

        public virtual string DominantColorForeground { get; set; }

        public virtual IList<string> DominantColors { get; set; }

        public virtual IList<string> ImageCategories { get; set; }

        public virtual bool IsAdultContent { get; set; }

        public virtual bool IsBwImg { get; set; }

        public virtual bool IsRacyContent { get; set; }

        public virtual string LineDrawingType { get; set; }

        public virtual IList<string> Tags { get; set; }
    }
}