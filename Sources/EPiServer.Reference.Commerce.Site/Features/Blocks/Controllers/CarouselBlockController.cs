using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Services;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true)]
    public class CarouselBlockController : BlockController<CarouselBlock>
    {
        private ContentLocator contentLocator;
        private IContentLoader contentLoader;
        public CarouselBlockController(ContentLocator contentLocator, IContentLoader contentLoader)
        {
            this.contentLocator = contentLocator;
            this.contentLoader = contentLoader;
        }

        public override ActionResult Index(CarouselBlock currentBlock)
        {
            if (currentBlock.MainContentArea == null) {
                return PartialView("CarouselBlock", null);
            }
            var model = new CarouselViewModel
            {
                Items = currentBlock.MainContentArea.FilteredItems.Select(
                        cai => contentLoader.Get<AlloyCarouselItemBlock>(cai.ContentLink)).ToList(),
                CurrentBlock = currentBlock
            };
            

            return PartialView("CarouselBlock", model);
        }

    }
}
