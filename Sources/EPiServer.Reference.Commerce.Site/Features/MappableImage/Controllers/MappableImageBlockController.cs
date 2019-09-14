using EPiServer.Reference.Commerce.Site.Features.MappableImage.Model;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.MappableImage.Controllers
{
    public class MappableImageBlockController : BlockController<MappableImageBlock>
    {
        public override ActionResult Index(MappableImageBlock currentBlock)
        {
            return PartialView("~/Views/Blocks/MappableImageBlock.cshtml", currentBlock);
        }
    }
}