﻿using EPiServer.Commerce.Order;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.B2B;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Manager.Order.CommandHandlers.PurchaseOrderHandlers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using System;
using System.Configuration;

namespace EPiServer.Reference.Commerce.Manager.CustomCommands
{
    public class FinishQuoteRequest : TransactionCommandHandler
    {
        protected override bool IsCommandEnable(IOrderGroup order, CommandParameters cp)
        {
            bool flag = base.IsCommandEnable(order, cp);
            if (flag && !string.IsNullOrEmpty(order.Properties[Constants.Quote.QuoteStatus] as string) )
                flag = order.Properties[Constants.Quote.QuoteStatus].ToString() == Constants.Quote.RequestQuotation;
            return flag;
        }

        protected override void DoCommand(IOrderGroup order, CommandParameters commandParameters)
        {
            try
            {
                PurchaseOrder purchaseOrder = order as PurchaseOrder;
                int quoteExpireDays;
                int.TryParse(ConfigurationManager.AppSettings[Constants.Quote.QuoteExpireDate], out quoteExpireDays);
                purchaseOrder[Constants.Quote.QuoteExpireDate] =
                    string.IsNullOrEmpty(ConfigurationManager.AppSettings[Constants.Quote.QuoteExpireDate])
                        ? DateTime.Now.AddDays(30)
                        : DateTime.Now.AddDays(quoteExpireDays);

                purchaseOrder[Constants.Quote.QuoteStatus] = Constants.Quote.RequestQuotationFinished;
                OrderStatusManager.ReleaseHoldOnOrder(purchaseOrder);
                AddNoteToOrder(purchaseOrder, "OrderNote_ChangeOrderStatusPattern", purchaseOrder.Status);
                SavePurchaseOrderChanges(purchaseOrder);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error("Failed to process request quote approve.", ex);
            }
        }
        
    }
}