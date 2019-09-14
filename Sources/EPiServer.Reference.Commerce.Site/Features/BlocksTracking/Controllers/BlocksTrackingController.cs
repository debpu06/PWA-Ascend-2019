using System.Web;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.BlocksTracking.Controllers
{
    public class BlocksTrackingController : Controller
    {
        private readonly IBlocksTrackingService _blocksTrackingService;
        private readonly HttpContextBase _httpContextBase;

        public BlocksTrackingController(IBlocksTrackingService blocksTrackingService, HttpContextBase httpContextBase)
        {
            _blocksTrackingService = blocksTrackingService;
            _httpContextBase = httpContextBase;
        }

        [HttpPost]
        public ActionResult TrackHeroBlock(string blockId, string blockName, string pageName)
        {
            _blocksTrackingService.TrackHeroBlock(_httpContextBase, blockId, blockName, pageName);
            return new ContentResult()
            {
                Content = blockName
            };
        }

        [HttpPost]
        public ActionResult TrackVideoBlock(string blockId, string blockName, string pageName)
        {
            _blocksTrackingService.TrackVideoBlock(_httpContextBase, blockId, blockName, pageName);
            return new ContentResult()
            {
                Content = blockName
            };
        }
    }
}