using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Indexing.Orders
{
    public class OrderIndexItem
    {
        public int PurchaseOrderId { get; set; }

        public DateTime Created { get; set; }            
        
        public string Country { get; set; }
        
        public string CurrencyCode { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string MarketId { get; set; }

        public DateTime? Modified { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public Guid? Organization { get; set; }

        public string OrganizationName { get; set; }

        public IEnumerable<string> Items { get; set; }

        public string OrderNumber { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string QuoteStatus { get; set; }

        public Guid? ParentOrganization { get; set; }

        public string ParentOrganizationName { get; set; }

        public decimal PreQuoteTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal HandlingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }
    }

    
}