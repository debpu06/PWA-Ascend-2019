using EPiServer.Reference.Commerce.Site.Features.Tracking.Models;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Tracking
{
    public class TrackingEventBlockController : BlockController<TrackingEventBlock>
    {
        public override ActionResult Index(TrackingEventBlock currentBlock)
        {
            return PartialView(currentBlock);
        }
    }
}