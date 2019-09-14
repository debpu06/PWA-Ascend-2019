using EPiServer.Commerce.Extensions;
using EPiServer.Commerce.Order;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using System;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Visitorgroups
{
    [VisitorGroupCriterion(LanguagePath = "/shared/itemsinnamedcart",
        DisplayName = "X Items in named cart",
        Description = "Find a user that has a specified x number in named cart",
        Category = "Commerce Criteria")]
    public class ItemsInNamedCart : CriterionBase<ItemsInNamedCartModel>
    {
        private readonly IOrderRepository _orderRepository;

        public ItemsInNamedCart()
        {
            _orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();
        }

        public override bool IsMatch(System.Security.Principal.IPrincipal principal, System.Web.HttpContextBase httpContext)
        {
            Guid userGuid = principal.GetUserGuid();
            if (userGuid == Guid.Empty)
            {
                return false;
            }

            var cart = _orderRepository.LoadCart<ICart>(userGuid, Model.CartName);
            if (cart == null || !cart.GetAllLineItems().Any())
            {
                return false;
            }

            return cart.GetAllLineItems().Count() >= Model.NumberOfItems;
        }
    }
}