using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.BlocksTracking
{
    public interface IBlocksTrackingService
    {
        void TrackHeroBlock(HttpContextBase context, string blockId, string blockName, string pageName);
        void TrackVideoBlock(HttpContextBase context, string blockId, string blockName, string pageName);
    }
}