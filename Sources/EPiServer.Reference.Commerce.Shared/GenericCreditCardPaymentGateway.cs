using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Plugins.Payment;

namespace EPiServer.Reference.Commerce.Shared
{
    public class GenericCreditCardPaymentGateway : AbstractPaymentGateway, IPaymentPlugin
    {
        /// <inheritdoc/>
        public override bool ProcessPayment(Payment payment, ref string message)
        {
            var result = ProcessPayment(null, payment);
            message = result.Message;
            return result.IsSuccessful;
        }

        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            var creditCardPayment = (ICreditCardPayment)payment;
            return !creditCardPayment.CreditCardNumber.EndsWith("4") ? PaymentProcessingResult.CreateUnsuccessfulResult("Invalid credit card number.") : 
                PaymentProcessingResult.CreateSuccessfulResult("");
        }
    }
}