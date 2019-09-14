using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Episerver.Marketing.Common.Helpers;
using Episerver.Marketing.Connector.Framework;
using Episerver.Marketing.Connector.Framework.Data;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons;
using EPiServer.Tracking.Core;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignMailId.Filters
{
    public class MailIdFilter : ActionFilterAttribute
    {
        public const string MailIdQsParamName = "mailid";

        private readonly IMailId _mailId;
        private readonly Lazy<ITrackingService> _trackingService;
        private readonly IMarketingConnectorManager _connectorManager;
        private IMarketingConnector _campaignConnector;
        private readonly ICookieHelper _cookieHelper;

        public MailIdFilter(IMailId mailId, Lazy<ITrackingService> trackingService, IMarketingConnectorManager connectorManager)
        {
            _mailId = mailId;
            _trackingService = trackingService;
            _connectorManager = connectorManager;
            // Seems the default ICookieHelper isn't registered in the container, not sure why
            _cookieHelper = new CookieHelper();
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            if (filterContext.HttpContext.Request.QueryString[MailIdQsParamName] != null)
            {
                // Get hold of the Campaign MA connector
                foreach (var connector in _connectorManager.GetActiveConnectors())
                {
                    // Note its assumed we have only one Campaign provider configured 
                    if (connector.Name.Contains("EPiServer Campaign"))
                    {
                        _campaignConnector = connector;
                        break;
                    }
                }

                var mailid = filterContext.HttpContext.Request.QueryString[MailIdQsParamName];

                // Look up the email address of the receipient from the mail id
                var email = _mailId.getRecipientId(mailid);
                var recipientListId = _mailId.getRecipientListId(mailid).ToString();
                if (!string.IsNullOrEmpty(email))
                {
                    // Set up the Campaign tracking cookie                
                    SetCampaignTrackingCookie(email, recipientListId);

                    // (optional) if Insight available then track here too
                    TrackInInsight(email, filterContext.HttpContext);
                }
            }
        }

        private void SetCampaignTrackingCookie(string email, string recipientListId)
        {
            // Use the cookie helper to drop the Campaign tracking cookie
            var datasourceId = recipientListId;
            var trackingCookie = this._cookieHelper.GetTrackingCookie(_campaignConnector.Id.ToString(),
                _campaignConnector.InstanceId.ToString());
            CookieData data = trackingCookie.FirstOrDefault(cd => cd.DatasourceId == datasourceId);
            if (data != null)
            {
                data.EntityId = email;
                data.DatasourceId = recipientListId;
            }
            else
            {
                var item = new CookieData
                {
                    DatasourceId = recipientListId,
                    EntityId = email
                };
                trackingCookie.Add(item);
            }
            _cookieHelper.UpsertTrackingCookie(
                _campaignConnector.Id.ToString(),
                _campaignConnector.InstanceId.ToString(), 
                trackingCookie);
        }

        private void TrackInInsight(string email, HttpContextBase httpContext)
        {
            try
            {
                // If you know the users name and/or additional information  
                // like company name it's also possible to associate here
                var userData = new UserData() { Email = email };
                var trackingData = new TrackingData<object>
                {
                    EventType = "EmailAssociated",
                    User = userData,
                    Value = "Associated email with current profile: '" + email + "'"
                };

                _trackingService.Value.Track(trackingData, httpContext);
            }
            catch
            {
                // ignored 
            }
        }
    }
}
