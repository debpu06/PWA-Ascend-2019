using System.Web.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true)]
    public class ImageGalleryBlockController : BlockController<ImageGalleryBlock>
    {
        public override ActionResult Index(ImageGalleryBlock currentBlock)
        {
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var images = repo.GetChildren<ImageMediaData>(currentBlock.Images);

            return PartialView(images);
        }
    }
}
