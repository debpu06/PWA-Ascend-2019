using EPiServer.ServiceLocation;
using EPiServer.Tracking.Core;
using System.Net.Mail;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.BlocksTracking.Services
{
    [ServiceConfiguration(typeof(IBlocksTrackingService))]
    public class BlocksTrackingService : IBlocksTrackingService
    {
        private readonly ITrackingService _trackingService;

        public BlocksTrackingService(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        public void TrackHeroBlock(HttpContextBase context, string blockId, string blockName, string pageName)
        {
            try
            {
                var trackingData = new TrackingData<dynamic>
                {
                    EventType = "epiHeroBlockClick",
                    Value = "Hero Block clicked: '" + blockName + "' on page - '" + pageName + "'",
                    PageUri = context.Request.UrlReferrer.AbsoluteUri,
                    PageTitle = pageName,
                    User = GetUserData(),
                    Payload = new
                    {
                        BlockId = blockId,
                        BlockName = blockName
                    }
                };

                _trackingService.Track(trackingData, context);
            }
            catch
            {
            }
        }

        public void TrackVideoBlock(HttpContextBase context, string blockId, string blockName, string pageName)
        {
            try
            {
                var trackingData = new TrackingData<dynamic>
                {
                    EventType = "epiVideoBlockView",
                    Value = "Video Block viewed: '" + blockName + "' on page - '" + pageName + "'",
                    PageUri = context.Request.UrlReferrer.AbsoluteUri,
                    PageTitle = pageName,
                    User = GetUserData(),
                    Payload = new
                    {
                        BlockId = blockId,
                        BlockName = blockName
                    }
                };

                _trackingService.Track(trackingData, context);
            }
            catch
            {
            }
        }

        private UserData GetUserData()
        {
            try
            {
                var current = Personalization.EPiServerProfile.Current;
                var email = new MailAddress(current.UserName);
                if (!string.IsNullOrEmpty(current.Email) || email.Address == current.UserName)
                {
                    var data = new UserData
                    {
                        Name = current != null ? current.UserName : "",
                        Email = current != null ? email.Address == current.UserName ? current.UserName : current.Email : ""
                    };
                    return data;
                }

                return null;
            }
            catch
            {
                return new UserData()
                {
                    Email = Personalization.EPiServerProfile.Current.Email,
                    Name = Security.PrincipalInfo.CurrentPrincipal.Identity.Name
                };
            }
        }
    }
}