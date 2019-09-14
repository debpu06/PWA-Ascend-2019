using System;
using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models
{
    /// <summary>Represents payment.</summary>
    [Serializable]
    public class Payment
    {
        /// <summary>Gets the payment id.</summary>
        /// <value>The payment id.</value>
        public int PaymentId { get; set; }

        /// <summary>Gets or sets the billing address id.</summary>
        /// <value>The billing address id.</value>
        public string BillingAddressId { get; set; }

        /// <summary>Gets or sets the payment method id.</summary>
        /// <value>The payment method id.</value>
        public Guid PaymentMethodId { get; set; }

        /// <summary>Gets or sets the name of the payment method.</summary>
        /// <value>The name of the payment method.</value>
        public string PaymentMethodName { get; set; }

        /// <summary>Gets or sets the name of the customer.</summary>
        /// <value>The name of the customer.</value>
        public string CustomerName { get; set; }

        /// <summary>Gets or sets the amount.</summary>
        /// <value>The amount.</value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the type of the payment. Types are CreditCard, CashCard, Invoice, GiftCard, Other
        /// </summary>
        /// <value>The type of the payment.</value>
        public PaymentType PaymentType { get; set; }

        /// <summary>Gets or sets the validation code.</summary>
        /// <value>The validation code.</value>
        public string ValidationCode { get; set; }

        /// <summary>Gets or sets the authorization code.</summary>
        /// <value>The authorization code.</value>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the status.</summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>Gets or sets the type of the transaction.</summary>
        /// <value>The type of the transaction. Mapped to TransactionType enumeration.</value>
        public string TransactionType { get; set; }

        /// <summary>Gets or sets the transaction ID.</summary>
        /// <value>The transaction ID.</value>
        public string TransactionID { get; set; }

        /// <summary>
        /// Gets or sets the transaction ID which is returned from Payment Provider.
        /// </summary>
        /// <value>The transaction ID.</value>
        public string ProviderTransactionID { get; set; }
    }
}