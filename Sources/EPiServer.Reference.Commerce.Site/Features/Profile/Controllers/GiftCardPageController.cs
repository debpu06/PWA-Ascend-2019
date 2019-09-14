using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Profile.Services;
using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Controllers
{
    /// <summary>
    /// A page to list all gift card belonging to a customer
    /// </summary>
    public class GiftCardPageController : PageController<GiftCardPage>
    {
        private readonly IGiftCardService _giftCardService;

        public GiftCardPageController(IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
        }

        public ActionResult Index(GiftCardPage currentPage)
        {
            var model = new GiftCardViewModel()
            {
                CurrentContent = currentPage,
                GiftCardList = _giftCardService.GetCustomerGiftCards(CustomerContext.Current.CurrentContactId.ToString()).ToList()
            };

            return View(model);
        }
    }
}