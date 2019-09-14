using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard
{
    public interface ICommerceDashboardService
    {
        IEnumerable<PurchaseOrder> SearchOrders(int start, int maxCount, SearchOrdersRequest request);
        IEnumerable<PurchaseOrder> GetOrders(int start, int maxCount);
        IEnumerable<ICart> GetCarts(int start, int maxCount);
        IEnumerable<Contact> GetContacts(int start, int maxCount);
        IEnumerable<object> GetRevenues();
        IEnumerable<object> GetProductsSold();
        IEnumerable<object> GetNewCustomers();
        IEnumerable<object> GetTopProducts();
        IEnumerable<object> GetAddedToCart();
        IEnumerable<object> GetTopPromotions();

    }
}
