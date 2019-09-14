using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Orders;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Controllers
{
    public class PaymentPlanHistoryController : PageController<PaymentPlanHistoryPage>
    {

        private readonly ICartService _cartService;
        private readonly IOrderRepository _orderRepository;
        private readonly CustomerContextFacade _customerContext;
        private readonly IContentLoader _contentLoader;

        public PaymentPlanHistoryController(
            ICartService cartService,
            IOrderRepository orderRepository,
            CustomerContextFacade customerContextFacade,
            IContentLoader contentLoader
            )
        {
            _cartService = cartService;
            _orderRepository = orderRepository;
            _customerContext = customerContextFacade;
            _contentLoader = contentLoader;
        }


        public ActionResult Index(PaymentPlanHistoryPage currentPage)
        {
            var paymentPlans = OrderContext.Current.LoadByCustomerId<PaymentPlan>(_customerContext.CurrentContactId)
                .OrderBy(x => x.Created)
                .ToList();

            var viewModel = new PaymentPlanHistoryViewModel()
            {
                CurrentContent = currentPage,
                PaymentPlans = paymentPlans
            };
            
            viewModel.PaymentPlanDetailsPageUrl = UrlResolver.Current.GetUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).PaymentPlanDetailsPage);

            return View(viewModel);
        }
    }
}