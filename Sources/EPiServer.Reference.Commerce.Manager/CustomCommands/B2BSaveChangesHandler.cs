using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.B2B;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Engine;
using Mediachase.Commerce.Manager.Apps.Core.CommandHandlers.Common;
using Mediachase.Commerce.Manager.Apps_Code.Order;
using Mediachase.Commerce.Manager.Order.CommandHandlers.PurchaseOrderHandlers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Manager.CustomCommands
{
    public class B2BSaveChangesHandler : SaveChangesHandler
    {
        protected override void DoCommand(IOrderGroup order, CommandParameters cp)
        {
            Mediachase.Ibn.Web.UI.CHelper.RequireDataBind();
            var purchaseOrder = order as PurchaseOrder;
            var workflowResults = OrderGroupWorkflowManager.RunWorkflow(purchaseOrder, "SaveChangesWorkflow",
                false,
                new Dictionary<string, object>
                {
                    {
                        "PreventProcessPayment",
                        !string.IsNullOrEmpty(order.Properties["QuoteStatus"] as string) &&
                        (order.Properties["QuoteStatus"].ToString() == Constants.Quote.RequestQuotation||
                        order.Properties["QuoteStatus"].ToString() == Constants.Quote.RequestQuotationFinished)
                    }
                });

            if (workflowResults.Status != WorkflowStatus.Completed)
            {
                string msg = "Unknow error";
                if (workflowResults.Exception != null)
                    msg = workflowResults.Exception.Message;
                ErrorManager.GenerateError(msg);
            }
            else
            {
                WriteOrderChangeNotes(purchaseOrder);
                SavePurchaseOrderChanges(purchaseOrder);
                OrderHelper.ExitPurchaseOrderFromEditMode(purchaseOrder.OrderGroupId);
            }

            var warnings = OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(workflowResults);
            if (warnings.Any())
            {
                CommandHandlerHelper.ShowStatusMessage(string.Join(", ", warnings), CommandManager);
            }
        }
    }
}