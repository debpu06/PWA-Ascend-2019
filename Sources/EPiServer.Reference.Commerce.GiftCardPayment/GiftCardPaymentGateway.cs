using System;
using EPiServer.Commerce.Order;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Plugins.Payment;

namespace EPiServer.Reference.Commerce.GiftCardPayment
{
    public class GiftCardPaymentGateway : AbstractPaymentGateway, IPaymentPlugin
    {
        public override bool ProcessPayment(Payment payment, ref string message)
        {
            var result = ProcessPayment(null, payment);
            message = result.Message;
            return result.IsSuccessful;
        }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            if (orderGroup == null)
            {
                return PaymentProcessingResult.CreateUnsuccessfulResult("Failed to process your payment.");
            }
            else
            {
                GiftCardManager.PurchaseByGiftCard(payment);
                return PaymentProcessingResult.CreateSuccessfulResult("Gift card processed");
            }
        }
    }
}
