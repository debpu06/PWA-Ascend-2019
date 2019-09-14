using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Internal;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Services
{
    [ServiceConfiguration(typeof(ICommerceDashboardService))]
    public class CommerceDashboardService : ICommerceDashboardService
    {
        private readonly IContentLoader _contentLoader;
        private readonly IOrderRepository _orderRepository;
        private readonly IPromotionEngine _promotionEngine;
        private readonly PromotionEngineContentLoader _promotionEngineContentLoader;
        private readonly PromotionInformationRepository _promotionInformationRepository;
        private readonly IOrderSearchService _orderSearchService;

        public CommerceDashboardService(
            IContentLoader contentLoader,
            IOrderRepository orderRepository,
            IPromotionEngine promotionEngine,
            PromotionEngineContentLoader promotionEngineContentLoader,
            PromotionInformationRepository promotionInformationRepository,
            IOrderSearchService orderSearchService)
        {
            _contentLoader = contentLoader;
            _orderRepository = orderRepository;
            _promotionEngine = promotionEngine;
            _promotionEngineContentLoader = promotionEngineContentLoader;
            _promotionInformationRepository = promotionInformationRepository;
            _orderSearchService = orderSearchService;
        }

        public IEnumerable<Models.PurchaseOrder> SearchOrders(int start, int maxCount, SearchOrdersRequest request)
        {
            try
            {
                var searchOptions = new OrderSearchOptions
                {
                    CacheResults = false,
                    StartingRecord = start,
                    RecordsToRetrieve = maxCount,
                    Namespace = "Mediachase.Commerce.Orders"
                };

                var parameters = new OrderSearchParameters();
                searchOptions.Classes.Add("PurchaseOrder");
                parameters.SqlMetaWhereClause = string.Empty;

                if (request?.ModifiedFrom.HasValue ?? false)
                {
                    parameters.SqlMetaWhereClause = $"META.Modified >= '{request.ModifiedFrom.Value:s}'";
                }

                if (request?.OrderShipmentStatus != null && request.ShippingMethodId != null && request.ShippingMethodId != Guid.Empty)
                {
                    parameters.SqlWhereClause =
                        $"[OrderGroupId] IN (SELECT [OrderGroupId] FROM [Shipment] WHERE [Status] = '{request.OrderShipmentStatus}' AND [ShippingMethodId] = '{request.ShippingMethodId}')";
                }
                else if (request?.OrderShipmentStatus != null)
                {
                    parameters.SqlWhereClause = $"[OrderGroupId] IN (SELECT [OrderGroupId] FROM [Shipment] WHERE [Status] = '{request.OrderShipmentStatus}')";
                }
                else if (request?.ShippingMethodId != null && request.ShippingMethodId != Guid.Empty)
                {
                    parameters.SqlWhereClause = $"[OrderGroupId] IN (SELECT [OrderGroupId] FROM [Shipment] WHERE [ShippingMethodId] = '{request.ShippingMethodId}')";
                }

                if (request != null && request.Status?.Length > 0)
                {
                    if (!string.IsNullOrEmpty(parameters.SqlWhereClause))
                    {
                        parameters.SqlWhereClause += " AND ";
                    }

                    var statusesParam = string.Join(",", request.Status.Select(x => $"'{x}'"));
                    parameters.SqlWhereClause += $"Status IN ({statusesParam})";
                }

                var orders = OrderContext.Current.Search<Mediachase.Commerce.Orders.PurchaseOrder>(parameters, searchOptions);
                return orders.Select(x => x.ConvertToPurchaseOrder()).ToArray();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error(ex.Message, ex.StackTrace);
                return null;
            }
        }

        public IEnumerable<Models.PurchaseOrder> GetOrders(int start, int maxCount)
        {
            try
            {
                var searchOptions = new OrderSearchOptions
                {
                    CacheResults = false,
                    StartingRecord = start,
                    RecordsToRetrieve = maxCount,
                    Namespace = "Mediachase.Commerce.Orders"
                };

                var parameters = new OrderSearchParameters();
                searchOptions.Classes.Add(OrderContext.PurchaseOrderClassType);
                parameters.SqlWhereClause = "OrderGroupId IN (SELECT OrdergroupId FROM Shipment)";

                var orders = OrderContext.Current.Search<Mediachase.Commerce.Orders.PurchaseOrder>(parameters, searchOptions);
                return orders.Select(x => x.ConvertToPurchaseOrder()).ToArray();
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error(ex.Message, ex.StackTrace);
                return null;
            }
        }

        public IEnumerable<ICart> GetCarts(int start, int maxCount)
        {
            try
            {
                var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var curEndDate = curStartDate.AddMonths(1).AddDays(-1);
                var orderFilter = new OrderSearchFilter
                {
                   StartingIndex = start,
                   RecordsToRetrieve = maxCount,
                   ModifiedFrom = curStartDate,
                   ModifiedTo = curEndDate
                };
                var searchResult = _orderSearchService.FindCarts(orderFilter);
                return searchResult.Orders;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error(ex.Message, ex.StackTrace);
                return null;
            }

        }

        public IEnumerable<Contact> GetContacts(int start, int maxCount)
        {
            IEnumerable<Contact> contacts;

            try
            {
                contacts = CustomerContext.Current.GetContacts(start, maxCount).Select(c => c.ConvertToContact());
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error(ex.Message, ex.StackTrace);
                return null;
            }

            return contacts;
        }

        public IEnumerable<object> GetRevenues()
        {
            var results = new List<object>();
            var orders = GetOrders(0, 10000);
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var curOrders = orders.Where(o => o.Created >= curStartDate && o.Created <= curEndDate)
                                .GroupBy(o => o.BillingCurrency)
                                .Select(g => new
                                {
                                    Currency = g.Key,
                                    Revenue = g.Sum(r => r.Total)
                                }).ToList();

            results.Add(new { Month = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Revenues = curOrders });

            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var preOrders = orders.Where(o => o.Created >= preStartDate && o.Created <= preEndDate)
                                .GroupBy(o => o.BillingCurrency)
                                .Select(g => new
                                {
                                    Currency = g.Key,
                                    Revenue = g.Sum(r => r.Total)
                                }).ToList();

            results.Add(new { Month = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Revenues = preOrders });

            return results;
        }

        public IEnumerable<object> GetProductsSold()
        {
            var results = new List<object>();
            var orders = GetOrders(0, 10000);
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var curProductsSold = orders.Where(o => o.Created >= curStartDate && o.Created <= curEndDate).Sum(o => o.OrderForms.Sum(of => of.LineItems.Count()));

            results.Add(new { Month = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), ProductsSold = curProductsSold });

            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var preProductsSold = orders.Where(o => o.Created >= preStartDate && o.Created <= preEndDate).Sum(o => o.OrderForms.Sum(of => of.LineItems.Count()));

            results.Add(new { Month = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), ProductsSold = preProductsSold });

            return results;
        }

        public IEnumerable<object> GetNewCustomers()
        {
            var results = new List<object>();
            var contacts = GetContacts(0, 10000);
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var curNewCustomers = contacts.Where(o => o.Created >= curStartDate && o.Created <= curEndDate).Count();

            results.Add(new { Month = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), NewCustomers = curNewCustomers });

            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var preNewCustomers = contacts.Where(o => o.Created >= preStartDate && o.Created <= preEndDate).Count();

            results.Add(new { Month = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), NewCustomers = preNewCustomers });

            return results;
        }

        public IEnumerable<object> GetTopProducts()
        {
            var results = new List<object>();
            var orders = GetOrders(0, 10000);
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var curOrders = orders.Where(o => o.Created >= curStartDate && o.Created <= curEndDate);
            var curLineItems = new List<object>();
            foreach (var curOrder in curOrders)
            {
                foreach (var of in curOrder.OrderForms)
                {
                    curLineItems.AddRange(of.LineItems.Select(o => new
                    {
                        o.Code,
                        o.DisplayName
                    }));
                }
            }

            var curTopProducts = curLineItems.GroupBy(o => o).Select(g => new { Product = g.Key, Count = g.Count() }).OrderByDescending(p => p.Count);

            results.Add(new { Month = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Products = curTopProducts });

            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var preOrders = orders.Where(o => o.Created >= preStartDate && o.Created <= preEndDate);
            var preLineItems = new List<object>();
            foreach (var preOrder in preOrders)
            {
                foreach (var of in preOrder.OrderForms)
                {
                    preLineItems.AddRange(of.LineItems.Select(o => new
                    {
                        o.Code,
                        o.DisplayName
                    }));
                }
            }

            var preTopProducts = preLineItems.GroupBy(o => o).Select(g => new { Product = g.Key, Count = g.Count() }).OrderByDescending(p => p.Count);

            results.Add(new { Month = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Products = preTopProducts });

            return results;
        }

        public IEnumerable<object> GetAddedToCart()
        {
            var results = new List<object>();
            var carts = GetCarts(0, 10000);
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var curProductsSold = carts.Where(o => o.Created >= curStartDate && o.Created <= curEndDate).Sum(o => o.Forms.Sum(of => of.Shipments.SelectMany(x => x.LineItems).Count()));

            results.Add(new { Month = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), ProductsSold = curProductsSold });

            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var preProductsSold = carts.Where(o => o.Created >= preStartDate && o.Created <= preEndDate).Sum(o => o.Forms.Sum(of => of.Shipments.SelectMany(x => x.LineItems).Count()));

            results.Add(new { Month = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), ProductsSold = preProductsSold });

            return results;
        }

        public IEnumerable<object> GetTopPromotions()
        {
            var promotions = _promotionEngineContentLoader.GetPromotions();
            var data = GetRedemptions(promotions);
            return data;
        }

        private IEnumerable<object> GetRedemptions(IEnumerable<PromotionData> promotions)
        {
            if (promotions == null || !promotions.Any())
            {
                return null;
            }

            var contentGuidIdMappings = promotions.Select(o => o.ContentLink).ToDictionary(x => _contentLoader.Get<IContent>(x).ContentGuid, x => x.ID);
            var redemptions = _promotionInformationRepository.GetRedemptions(contentGuidIdMappings.Keys);

            return redemptions.Select(x => new
            {
                Promotion = promotions.FirstOrDefault(o => o.ContentGuid == x.PromotionGuid).Name,
                DiscountType = promotions.FirstOrDefault(o => o.ContentGuid == x.PromotionGuid).DiscountType.ToString(),
                x.TotalRedemptions
            }).OrderByDescending(x => x.TotalRedemptions);
        }
    }
}