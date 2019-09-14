using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.MappableImage.Model
{
    [ContentType(DisplayName = "Mappable Image", GUID = "1b24ea46-2036-40cd-9309-3d1c2f3a9bc0", Description = "")]
    [SiteImageUrl("~/content/icons/blocks/CMS-icon-block-04.png")]
    public class MappableImageBlock : BlockData
    {
        // I'm really only doing this for that sweet, sweet OPE capabilities
        public virtual MappableImageBlockProperties Properties { get; set; }
    }

    [ContentType(DisplayName = "Mappable Image Properties", GUID = "F5EC6630-77A6-43A3-A9D5-B35097FB3C34", Description = "", AvailableInEditMode = false)]
    public class MappableImageBlockProperties : BlockData
    {
        [Display(Name = "Background Image")]
        [UIHint(UIHint.Image)]
        public virtual ContentReference BackgroundImageContentReference { get; set; }

        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<MappableCoordinate>))]
        public virtual IList<MappableCoordinate> MappableCoordinateList { get; set; }
    }
}