using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using Mediachase.Commerce.Customers;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Services
{
    /// <summary>
    /// All action on credit card data
    /// </summary>
    public interface ICreditCardService
    {
        /// <summary>
        /// List all credit card that avaiable for user or organization
        /// </summary>
        /// <param name="isOrganization">List for Organization or not</param>
        /// <param name="isUsingToPurchase">List credit card to manage or to purchase
        /// In case manager: user only see own credit card or organization's card depend on setting isOrganization
        /// In case purchase: user can use own credit card and card of organization that user is belong
        /// </param>
        /// <returns></returns>
        IList<CreditCardModel> List(bool isOrganization = false, bool isUsingToPurchase = false);

        /// <summary>
        /// Check credit card is valid for edit/delete
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <param name="errorMessage">Error message when credit card id is not valid</param>
        /// <returns></returns>
        bool IsValid(string creditCardId, out string errorMessage);

        /// <summary>
        /// Check credit card is valid to use
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        bool IsReadyToUse(string creditCardId);

        /// <summary>
        /// Save credit card
        /// </summary>
        /// <param name="creditCardModel">Model of credit card</param>
        void Save(CreditCardModel addressModel);

        /// <summary>
        /// Delete a credit card
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        void Delete(string creditCardId);

        /// <summary>
        /// Load data for a credit card
        /// </summary>
        /// <param name="creditCardModel">Model of credit card</param>
        void LoadCreditCard(CreditCardModel creditCardModel);

        /// <summary>
        /// Get credit card by id
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        CreditCard GetCreditCard(string creditCardId);

        /// <summary>
        /// Map credit card view model to credit card of commerce core
        /// </summary>
        /// <param name="customerCreditCard">Source credit card</param>
        /// <param name="creditCard">Target credit card</param>
        void MapToCreditCard(CreditCardModel creditCard, ref CreditCard orderAddress);

        /// <summary>
        /// Map credit card of commerce core to credit card view model
        /// </summary>
        /// <param name="creditCard">Source credit card</param>
        /// <param name="customerCreditCard">Target credit card</param>
        void MapToModel(CreditCard customerAddress, ref CreditCardModel addressModel);

    }
}
