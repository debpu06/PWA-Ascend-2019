using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.Enums;
using EPiServer.Reference.Commerce.B2B.Models.Entities;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Plugins.Payment;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.BudgetPayment
{
    public class BudgetPaymentGateway : AbstractPaymentGateway, IPaymentPlugin
    {
        private static Injected<IBudgetService> _budgetService = default(Injected<IBudgetService>);
        private static Injected<ICustomerService> _customerService = default(Injected<ICustomerService>);
        private static Injected<IOrdersService> _ordersService = default(Injected<IOrdersService>);
        private static Injected<IOrderRepository> _orderRepository = default(Injected<IOrderRepository>);
        public IOrderGroup OrderGroup { get; set; }

        public override bool ProcessPayment(Payment payment, ref string message)
        {
            var result = ProcessPayment(OrderGroup, payment);
            message = result.Message;
            return result.IsSuccessful;
        }
        
        private void UpdateUserBudgets(ContactViewModel customer, decimal amount)
        {
            var budgetsToUpdate = new List<Budget>
            {
                _budgetService.Service.GetCurrentOrganizationBudget(customer.Organization.OrganizationId),
                _budgetService.Service.GetCurrentOrganizationBudget(customer.Organization.ParentOrganizationId),
                _budgetService.Service.GetCustomerCurrentBudget(customer.Organization.OrganizationId,
                    customer.ContactId)
            }.Where(x => x != null).ToList();

            if (budgetsToUpdate.All(budget => budget == null)) return;

            foreach (var budget in budgetsToUpdate)
            {
                budget.SpentBudget += amount;
                budget.SaveChanges();
            }
        }

        private bool AreBudgetsOnHold(ContactViewModel customer)
        {
            if (customer?.Organization == null) return true;

            var budgetsToCheck = new List<Budget>
            {
                _budgetService.Service.GetCurrentOrganizationBudget(customer.Organization.OrganizationId),
                _budgetService.Service.GetCurrentOrganizationBudget(customer.Organization.ParentOrganizationId),
                _budgetService.Service.GetCustomerCurrentBudget(customer.Organization.OrganizationId,
                    customer.ContactId)
            }.Where(x => x != null);
            return budgetsToCheck.Any(budget => budget.Status.Equals(Constants.BudgetStatus.OnHold));
        }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            if (orderGroup == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("Failed to process your payment.");
            }

            var currentOrder = orderGroup;
            var customer = _customerService.Service.GetContactById(currentOrder.CustomerId.ToString());
            if (customer == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("Failed to process your payment.");
            }
            var isQuoteOrder = currentOrder.Properties[Constants.Quote.ParentOrderGroupId] != null && 
                Convert.ToInt32(currentOrder.Properties[Constants.Quote.ParentOrderGroupId]) != 0;
            if (isQuoteOrder)
            {
                if (customer.Role != B2BUserRoles.Approver)
                {
                    return PaymentProcessingResult.CreateUnsuccessfulResult("Failed to process your payment.");
                }
            }
            

            var purchaserCustomer = !isQuoteOrder ? customer : _ordersService.Service.GetPurchaserCustomer(currentOrder);
            if (AreBudgetsOnHold(purchaserCustomer))
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("Budget on hold.");
            }

            if (customer.Role == B2BUserRoles.Purchaser)
            {
                var budget = _budgetService.Service.GetCustomerCurrentBudget(purchaserCustomer.Organization.OrganizationId, purchaserCustomer.ContactId);
                if (budget == null || budget.RemainingBudget < payment.Amount)
                {
                    return PaymentProcessingResult.CreateUnsuccessfulResult("Insufficient budget.");
                }
            }
            

            if (payment.TransactionType == TransactionType.Capture.ToString())
            {
                UpdateUserBudgets(purchaserCustomer, payment.Amount);
                payment.Status = PaymentStatus.Processed.ToString();
                _orderRepository.Service.Save(currentOrder);
            }
            return PaymentProcessingResult.CreateSuccessfulResult("");
        }
    }
}
