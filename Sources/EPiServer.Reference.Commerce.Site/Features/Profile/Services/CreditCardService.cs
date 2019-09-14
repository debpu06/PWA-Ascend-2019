using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.B2B.DomainServices;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.B2B.Services;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation.Data;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce.Customers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EPiServer.Reference.Commerce.Site.Features.Profile.Services
{
    /// <summary>
    /// All action on credit card data
    /// </summary>
    [ServiceConfiguration(typeof(ICreditCardService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CreditCardService : ICreditCardService
    {

        private readonly CustomerContextFacade _customerContext;
        private readonly IOrganizationService _organizationService;
        private readonly OrganizationDomainService _organizationDomainService;
        private readonly CustomerService _customerService;
        private readonly LocalizationService _localizationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerContext"></param>
        /// <param name="organizationService"></param>
        /// <param name="organizationDomainService"></param>
        /// <param name="customerService"></param>
        public CreditCardService(
            CustomerContextFacade customerContext,
            IOrganizationService organizationService,
            OrganizationDomainService organizationDomainService,
            CustomerService customerService,
            LocalizationService localizationService
        )
        {
            _customerContext = customerContext;
            _organizationService = organizationService;
            _organizationDomainService = organizationDomainService;
            _customerService = customerService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Check credit card is valid for edit/delete
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <param name="errorMessage">Error message when credit card id is not valid</param>
        /// <returns></returns>
        public bool IsValid(string creditCardId, out string errorMessage)
        {
            errorMessage = null;

            //AddNew
            if (String.IsNullOrEmpty(creditCardId))
                return true;

            //Delete, Edit
            var currentCreditCard = GetCreditCard(creditCardId);
            var currentUser = _customerService.GetCurrentContact();
            
            if (currentCreditCard != null)
            {
                if (currentCreditCard.ContactId == currentUser.ContactId)
                {
                    return true;
                }
                else if (currentUser.IsAdmin)
                {
                    var currentOrganization = _organizationService.GetCurrentUserOrganization();
                    if (IsValidOrganizationCard(currentCreditCard, currentOrganization))
                    {
                        return true;
                    }
                }
            }

            errorMessage = _localizationService.GetString("/CreditCard/ValidationErrors/InvalidCreditCard");

            return false;
        }

        /// <summary>
        /// Check credit card of organization is valid for edit/delete
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        private bool IsValidOrganizationCard(CreditCard creditCard, OrganizationModel organization)
        {
            if (organization.OrganizationId == organization.OrganizationId)
            {
                return true;
            }
            else
            {
                var isValid = false;

                foreach(var subOrganization in organization.SubOrganizations)
                {
                    if (IsValidOrganizationCard(creditCard, subOrganization))
                    {
                        isValid = true;
                        break;
                    }
                }

                return isValid;
            }

        }


        /// <summary>
        /// Check credit card is valid to use
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        public bool IsReadyToUse(string creditCardId)
        {
            if (String.IsNullOrEmpty(creditCardId))
                return false;

            var curCreditCard = GetCreditCard(creditCardId);
            if (curCreditCard == null)
            {
                return false;
            }
            else
            {
                var currentUser = _customerService.GetCurrentContact();
                if (curCreditCard.ContactId == currentUser.ContactId)
                {
                    return true;
                }
                else
                {
                    var currentOrganization = _organizationService.GetCurrentUserOrganization();
                    if (currentOrganization != null && curCreditCard.OrganizationId == currentOrganization.OrganizationId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Delete a credit card
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        public void Delete(string creditCardId)
        {
            string errorMessage;
            if (IsValid(creditCardId, out errorMessage))
            {
                try
                {
                    CreditCard.Delete(PrimaryKeyId.Parse(creditCardId));
                }
                catch
                {
                    //do nothing
                }
            }
        }

        /// <summary>
        /// Save credit card
        /// </summary>
        /// <param name="creditCardModel">Model of credit card</param>
        public void Save(CreditCardModel creditCardModel)
        {
            string errorMessage;
            if (IsValid(creditCardModel.CreditCardId, out errorMessage))
            {
                var creditCard = GetCreditCard(creditCardModel.CreditCardId);
                var isNew = creditCard == null;

                if (isNew)
                {
                    creditCard = CreditCard.CreateInstance();
                }

                MapToCreditCard(creditCardModel, ref creditCard);

                if (isNew)
                {
                    //Create CC for user
                    if (creditCard.OrganizationId == null)
                    {
                        creditCard.ContactId = PrimaryKeyId.Parse(_customerService.GetCurrentContact().ContactId.ToString());
                    }
                    BusinessManager.Create(creditCard);
                }
                else
                {
                    BusinessManager.Update(creditCard);
                }
            }
        }

        /// <summary>
        /// List all credit card that avaiable for user or organization
        /// </summary>
        /// <param name="isOrganization">List for Organization or not</param>
        /// <param name="isUsingToPurchase">List credit card to manage or to purchase
        /// In case manager: user only see own credit card or organization's card depend on setting isOrganization
        /// In case purchase: user can use own credit card and card of organization that user is belong
        /// </param>
        /// <returns></returns>
        public IList<CreditCardModel> List(bool isOrganization = false, bool isUsingToPurchase = false)
        {
            var currentContact = _customerContext.CurrentContact;
            var creditCards = new List<CreditCardModel>();

            //Get credit card of current contact
            if (currentContact != null && !isOrganization)
            {
                AddRangeCreditCard(currentContact.CurrentContact, null, creditCards, currentContact.ContactCreditCards);
            }

            if (isUsingToPurchase || isOrganization)
            {
                //Get credit card of all organization that current customer belong
                var currentOrganization = _organizationDomainService.GetCurrentUserOrganizationEntity();
                GetCreditCardOrganization(currentOrganization, !isUsingToPurchase, creditCards);
            }
           
            return creditCards;
        }


        /// <summary>
        /// Get all credit card of current organization and its sub organization
        /// </summary>
        /// <param name="organization">Organization that need to get credit card from</param>
        /// <returns></returns>
        private void GetCreditCardOrganization(B2B.Models.Entities.B2BOrganization organization, bool recursive, List<CreditCardModel> list)
        {
            if (organization != null)
            {
                var orgCards = _customerContext.GetCreditCardsForOrganization(organization.OrganizationEntity);
                AddRangeCreditCard(null, new OrganizationModel(organization), list, orgCards);

                if (organization.SubOrganizations.Count > 0 && recursive)
                {
                    foreach (var subOrg in organization.SubOrganizations)
                    {
                        GetCreditCardOrganization(subOrg, recursive, list);
                    }
                }
            }
        }

        /// <summary>
        /// Load data for a credit card
        /// </summary>
        /// <param name="creditCardModel">Model of credit card</param>
        public void LoadCreditCard(CreditCardModel creditCardModel)
        {
            CreditCard creditCard = GetCreditCard(creditCardModel.CreditCardId);
            if (creditCard != null)
            {
                MapToModel(creditCard, ref creditCardModel);
            }
        }

        /// <summary>
        /// Map credit card view model to credit card of commerce core
        /// </summary>
        /// <param name="customerCreditCard">Source credit card</param>
        /// <param name="creditCard">Target credit card</param>
        public void MapToCreditCard(CreditCardModel customerCreditCard, ref CreditCard creditCard)
        {
            creditCard.CardType = (int)customerCreditCard.CreditCardType;
            creditCard.CreditCardNumber = customerCreditCard.CreditCardNumber;
            creditCard.LastFourDigits =
                customerCreditCard.CreditCardNumber.Substring(customerCreditCard.CreditCardNumber.Length - 4);
            creditCard.SecurityCode = customerCreditCard.CreditCardSecurityCode;
            creditCard.ExpirationMonth = customerCreditCard.ExpirationMonth;
            creditCard.ExpirationYear = customerCreditCard.ExpirationYear;
            if (customerCreditCard.CurrentContact != null)
            {
                creditCard.ContactId = customerCreditCard.CurrentContact.PrimaryKeyId;
            }
            else if (!String.IsNullOrEmpty(customerCreditCard.OrganizationId))
            {
                creditCard.OrganizationId =
                    PrimaryKeyId.Parse(customerCreditCard.OrganizationId);
            }
            if (!string.IsNullOrEmpty(customerCreditCard.CreditCardId))
            {
                creditCard.PrimaryKeyId = PrimaryKeyId.Parse(customerCreditCard.CreditCardId);
            }
        }

        /// <summary>
        /// Map credit card of commerce core to credit card view model
        /// </summary>
        /// <param name="creditCard">Source credit card</param>
        /// <param name="customerCreditCard">Target credit card</param>
        public void MapToModel(CreditCard creditCard, ref CreditCardModel customerCreditCard)
        {
            customerCreditCard.CreditCardType = (CreditCard.eCreditCardType) creditCard.CardType;
            customerCreditCard.CreditCardNumber = creditCard.CreditCardNumber;
            customerCreditCard.CreditCardSecurityCode = creditCard.SecurityCode;
            customerCreditCard.ExpirationMonth = creditCard.ExpirationMonth;
            customerCreditCard.ExpirationYear = creditCard.ExpirationYear;
            customerCreditCard.CreditCardId = creditCard.PrimaryKeyId.ToString();

            if (creditCard.OrganizationId != null)
            {
                customerCreditCard.Organization = new OrganizationModel(_organizationDomainService.GetOrganizationEntityById(creditCard.OrganizationId.ToString()));
            }
            else if (creditCard.ContactId != null)
            {
                customerCreditCard.CurrentContact = _customerContext.GetContactById(new Guid(creditCard.ContactId.ToString()));
            }
        }

        /// <summary>
        /// Get credit card by id
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        public CreditCard GetCreditCard(string creditCardId)
        {
            if (string.IsNullOrEmpty(creditCardId)) return null;

            return Enumerable.OfType<CreditCard>(BusinessManager.List(
                CreditCardEntity.ClassName, new FilterElement[1]
                {
                    new FilterElement("CreditCardId", FilterElementType.Equal, new Guid(creditCardId))
                })).FirstOrDefault();
        }

        /// <summary>
        /// Append a list of credit card to current credit card
        /// </summary>
        /// <param name="customerContact"></param>
        /// <param name="organization"></param>
        /// <param name="currentListCards"></param>
        /// <param name="appendListCreditCards"></param>
        private void AddRangeCreditCard(CustomerContact customerContact, OrganizationModel organization, List<CreditCardModel> currentListCards, IEnumerable<CreditCard> appendListCreditCards)
        {
            currentListCards.AddRange(appendListCreditCards.Select(x => new CreditCardModel
            {
                CreditCardNumber = x.CreditCardNumber,
                CreditCardType = (CreditCard.eCreditCardType)x.CardType,
                CreditCardSecurityCode = x.SecurityCode,
                ExpirationMonth = x.ExpirationMonth,
                ExpirationYear = x.ExpirationYear,
                CreditCardId = x.PrimaryKeyId.ToString(),
                CurrentContact = customerContact,
                Organization = organization
            }));
        }

    }
}