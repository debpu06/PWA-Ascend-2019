using EPiServer.Reference.Commerce.Site.Features.Profile.Models;
using EPiServer.Reference.Commerce.Site.Features.Profile.Services;
using EPiServer.Shell.Navigation;
using Mediachase.Commerce.Customers;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.GiftCards.Controllers
{
    public class GiftCardManagerController : Controller
    {
        private readonly IGiftCardService _giftCardService;

        public GiftCardManagerController(IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
        }

        [MenuItem("/global/commerce/giftcards", TextResourceKey = "/Shared/GiftCards")]
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult GetAllGiftCards()
        {
            var data = _giftCardService.GetAllGiftCards();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        [HttpPost]
        public string AddGiftCard(GiftCard giftCard)
        {
            return _giftCardService.CreateGiftCard(giftCard);
        }

        [HttpPost]
        public string UpdateGiftCard(GiftCard giftCard)
        {
            return _giftCardService.UpdateGiftCard(giftCard);
        }

        [HttpPost]
        public string DeleteGiftCard(string giftCardId)
        {
            return _giftCardService.DeleteGiftCard(giftCardId);
        }

        [HttpPost]
        public ContentResult GetAllContacts()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var data = CustomerContext.Current.GetContacts().Select(c => new
#pragma warning restore CS0618 // Type or member is obsolete
            {
                ContactId = c.PrimaryKeyId.ToString(),
                ContactName = c.FullName
            });

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }
    }
}