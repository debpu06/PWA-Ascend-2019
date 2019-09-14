using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.ViewModels;


namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Controllers
{
    public class MoseySupplyMarketController : Controller
    {
        private readonly IMarketService _marketService;
        private readonly ICurrentMarket _currentMarket;
        private readonly UrlResolver _urlResolver;
        private readonly LanguageService _languageService;
        private readonly ICartService _cartService;
        private readonly ICurrencyService _currencyService;
        private const string FlagLocation = "/Assets/Mosey/images/flags/";

        public MoseySupplyMarketController(
            IMarketService marketService,
            ICurrentMarket currentMarket,
            UrlResolver urlResolver,
            LanguageService languageService,
            ICartService cartService,
            ICurrencyService currencyService)
        {
            _marketService = marketService;
            _currentMarket = currentMarket;
            _urlResolver = urlResolver;
            _languageService = languageService;
            _cartService = cartService;
            _currencyService = currencyService;
        }

        [ChildActionOnly]
        public ActionResult Index(ContentReference contentLink)
        {
            var currentMarket = _currentMarket.GetCurrentMarket();
            var model = new MarketViewModel
            {
                Markets = _marketService.GetAllMarkets().Where(x => x.IsEnabled).OrderBy(x => x.MarketName)
                    .Select(x => new MarketItem
                    {
                        Selected = false,
                        Text = x.MarketName,
                        Value = x.MarketId.Value,
                        FlagUrl = GetFlagUrl(x.MarketId)
                    }),
                MarketId = currentMarket != null ? currentMarket.MarketId.Value : string.Empty,
                CurrentMarket = currentMarket != null ? GetFlagUrl(currentMarket.MarketId) : string.Empty,
                ContentLink = contentLink
            };
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Set(string marketId, ContentReference contentLink)
        {
            var newMarketId = new MarketId(marketId);
            _currentMarket.SetCurrentMarket(newMarketId);
            var currentMarket = _marketService.GetMarket(newMarketId);
            var cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart;

            if (cart != null && cart.Currency != null)
            {
                _currencyService.SetCurrentCurrency(cart.Currency);
            }
            else
            {
                _currencyService.SetCurrentCurrency(currentMarket.DefaultCurrency);
            }

            _languageService.UpdateLanguage(currentMarket.DefaultLanguage.Name);

            var returnUrl = _urlResolver.GetUrl(Request, contentLink, currentMarket.DefaultLanguage.Name);
            return Json(new { returnUrl });
        }

        private string GetFlagUrl(MarketId marketId)
        {
            if (marketId == new MarketId("FR"))
            {
                return $"{FlagLocation}fr.svg";
            }
            if (marketId == new MarketId("AUS"))
            {
                return $"{FlagLocation}au.svg";
            }
            if (marketId == new MarketId("BRA"))
            {
                return $"{FlagLocation}br.svg";
            }
            if (marketId == new MarketId("CAN"))
            {
                return $"{FlagLocation}ca.svg";
            }
            if (marketId == new MarketId("CHL"))
            {
                return $"{FlagLocation}cl.svg";
            }

            if (marketId == new MarketId("DEFAULT"))
            {
                return $"{FlagLocation}us.svg";
            }

            if (marketId == new MarketId("DEU"))
            {
                return $"{FlagLocation}de.svg";
            }
            if (marketId == new MarketId("ESP"))
            {
                return $"{FlagLocation}es.svg";
            }
            if (marketId == new MarketId("JPN"))
            {
                return $"{FlagLocation}jp.svg";
            }
            if (marketId == new MarketId("NLD"))
            {
                return $"{FlagLocation}nl.svg";
            }
            if (marketId == new MarketId("NOR"))
            {
                return $"{FlagLocation}no.svg";
            }
            if (marketId == new MarketId("SAU"))
            {
                return $"{FlagLocation}sa.svg";
            }
            if (marketId == new MarketId("SWE"))
            {
                return $"{FlagLocation}se.svg";
            }
            if (marketId == new MarketId("UK"))
            {
                return $"{FlagLocation}gb.svg";
            }
            return marketId == new MarketId("US") ? $"{FlagLocation}us.svg" : "";
        }
    }
}