using EPiServer.Commerce.Order;
using EPiServer.Find;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using Mediachase.Commerce.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Indexing.Orders
{
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class OrderIndexInitialization : IInitializableModule
    {
        private IClient _client;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(OrderIndexInitialization));
        private CustomerContextFacade _customerContext;

        public void Initialize(InitializationEngine context)
        {
            _client = context.Locate.Advanced.GetInstance<IClient>();
            _customerContext = context.Locate.Advanced.GetInstance<CustomerContextFacade>();
            //OrderContext.Current.OrderGroupUpdated += Current_OrderGroupUpdated;
            //OrderContext.Current.OrderGroupDeleted += Current_OrderGroupDeleted;
        }

        

        public void Uninitialize(InitializationEngine context)
        {
            //OrderContext.Current.OrderGroupUpdated -= Current_OrderGroupUpdated;
            //OrderContext.Current.OrderGroupDeleted -= Current_OrderGroupDeleted;
        }

        private void Current_OrderGroupUpdated(object sender, OrderGroupEventArgs e)
        {
            if (e.OrderGroupType != OrderGroupEventType.PurchaseOrder)
            {
                return;
            }
            Task.Run(() =>
            {
                var document = GetItem(sender as PurchaseOrder);
                if (document == null)
                {
                    return;
                }
                var result = _client.Index(document);
                if (!result.Ok)
                {
                    _logger.Information("Failed to index order");
                }
            });
        }

        private void Current_OrderGroupDeleted(object sender, OrderGroupEventArgs e)
        {
            
        }

        private OrderIndexItem GetItem(PurchaseOrder order)
        { 
            if (order == null)
            {
                return null;
            }

            var contact = _customerContext.GetContactById(order.CustomerId);
            return new OrderIndexItem
            {
                Country = GetCountry(order),
                Created = order.Created,
                CurrencyCode = order.BillingCurrency,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                Email = contact.Email
            };
        }

        public static string GetCountry(PurchaseOrder order)
        {
            var address = order.GetFirstForm().Shipments.Select(x => x.ShippingAddress).FirstOrDefault();
            return address == null ? string.Empty : address.CountryName;
        }

        public static string GetEmail(PurchaseOrder order)
        {
            var address = order.GetFirstForm().Shipments.Select(x => x.ShippingAddress).FirstOrDefault();
            return address == null ? string.Empty : address.Email;
        }
    }
}