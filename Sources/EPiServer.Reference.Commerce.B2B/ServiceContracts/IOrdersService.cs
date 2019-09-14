using System;
using System.Collections.Generic;
using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.B2B.ServiceContracts
{
    public interface IOrdersService
    {
        List<OrderOrganizationViewModel> GetUserOrders(Guid userGuid);
        IPayment GetOrderBudgetPayment(IPurchaseOrder purchaseOrder);
        bool ApproveOrder(int orderGroupId);
        ContactViewModel GetPurchaserCustomer(IOrderGroup order);

        /// <summary>
        /// Create a return order
        /// </summary>
        /// <param name="orderGroupId"></param>
        /// <param name="shipmentId"></param>
        /// <param name="lineItemId"></param>
        /// <param name="returnQuantity"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        ReturnFormStatus CreateReturn(int orderGroupId, int shipmentId, int lineItemId, decimal returnQuantity, string reason);

        Dictionary<ILineItem, List<ValidationIssue>> ChangeLineItemPrice(int orderGroupId, int shipmentId, int lineItemId, decimal price);
        Dictionary<ILineItem, List<ValidationIssue>> ChangeLineItemQuantity(int orderGroupId, int shipmentId, int lineItemId, decimal quantity);
        void AddNote(IPurchaseOrder order, string title, string detail);
    }
}
