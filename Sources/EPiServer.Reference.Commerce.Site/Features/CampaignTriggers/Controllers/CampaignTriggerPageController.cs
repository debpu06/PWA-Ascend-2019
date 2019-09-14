using EPiServer.ConnectForCampaign.Core.Configuration;
using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Pages;
using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Services;
using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.ViewModels;
using EPiServer.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Controllers
{
    public class CampaignTriggerPageController : PageController<CampaignTriggerPage>
    {
        private readonly ICampaignTriggerService _campaignTriggerService;

        private readonly ICampaignToken _campaignToken;

        public CampaignTriggerPageController(ICampaignTriggerService campaignTriggerService, ICampaignToken campaignToken)
        {
            _campaignTriggerService = campaignTriggerService;
            _campaignToken = campaignToken;
        }

        public ActionResult Index(CampaignTriggerPage currentPage)
        {
            var model = new CampaignTriggerViewModel(currentPage);
            return View(model);
        }

        [HttpPost]
        public async Task<string> SendTestMailsForTriggers(int trigger,string mailingId, string recipientListId, string emailList)
        {
            char[] splitter = new char[] { ';', ' ', ',' };
            var recipientId = emailList.Split(splitter);
            var result = new int[0];
            switch (trigger)
            {
                case (int)TriggerId.AbandonedBasket:
                    result = await _campaignTriggerService.SendTestMails("208892186976", recipientListId, recipientId);
                    break;
                case (int)TriggerId.Purchased3Days:
                    result = await _campaignTriggerService.SendTestMails("209861741633", recipientListId, recipientId);
                    break;
                case (int)TriggerId.AbandonedBrowse:
                    result = await _campaignTriggerService.SendTestMails("210396047224", recipientListId, recipientId);
                    break;
                case (int)TriggerId.NoLogin:
                    result = await _campaignTriggerService.SendTestMails("209657311358", recipientListId, recipientId);
                    break;
                case (int)TriggerId.SignUpLoyalty:
                    result = await _campaignTriggerService.SendTestMails("210721673718", recipientListId, recipientId);
                    break;
                case (int)TriggerId.InactiveCustomer:
                    result = await _campaignTriggerService.SendTestMails("215785112979", recipientListId, recipientId);
                    break;
                default:
                    result = await _campaignTriggerService.SendTestMails(mailingId, recipientListId, recipientId);
                    break;
            }

            var sb = new StringBuilder();
            sb.Append("Cannot send to email(s):");
            bool allSuccess = true;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != 0)
                {
                    allSuccess = false;
                    sb.Append(recipientId[i] + "; ");
                }
            }

            if (allSuccess)
            {
                return "Sent succesfully";
            }
            else
            {
                return sb.ToString();
            }
        }
    }
}