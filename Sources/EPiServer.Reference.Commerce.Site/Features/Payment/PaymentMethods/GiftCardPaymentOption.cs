﻿using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using EPiServer.Framework.Localization;
using Mediachase.Commerce;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Payment.Services;
using Mediachase.Commerce.Orders;
using EPiServer.Reference.Commerce.Site.Features.Profile.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Profile.Services;
using Mediachase.Commerce.Customers;
using EPiServer.Reference.Commerce.GiftCardPayment;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods
{
    [ServiceConfiguration(typeof(IPaymentMethod))]
    public class GiftCardPaymentOption : PaymentOptionBase
    {
        private readonly IOrderGroupFactory _orderGroupFactory;
        Injected<IGiftCardService> _giftCardService;

        public List<SelectListItem> AvailableGiftCards { get; set; }
        public string SelectedGiftCardId { get; set; }

        public GiftCardPaymentOption()
            : this(LocalizationService.Current, ServiceLocator.Current.GetInstance<IOrderGroupFactory>(), ServiceLocator.Current.GetInstance<ICurrentMarket>(), ServiceLocator.Current.GetInstance<LanguageService>(), ServiceLocator.Current.GetInstance<IPaymentService>())
        {
        }

        public GiftCardPaymentOption(LocalizationService localizationService,
            IOrderGroupFactory orderGroupFactory,
            ICurrentMarket currentMarket,
            LanguageService languageService,
            IPaymentService paymentService)
            : base(localizationService, orderGroupFactory, currentMarket, languageService, paymentService)
        {
            _orderGroupFactory = orderGroupFactory;

            AvailableGiftCards = new List<SelectListItem>();
            var isActiveGiftCards = _giftCardService.Service.GetCustomerGiftCards(CustomerContext.Current.CurrentContactId.ToString()).Where(g => g.IsActive == true);
            
            foreach (var giftCard in isActiveGiftCards)
            {
                AvailableGiftCards.Add(new SelectListItem()
                {
                    Text = giftCard.GiftCardName + " - " + giftCard.RemainBalance + " point(s)",
                    Value = giftCard.GiftCardId
                });
            }
        }

        public override string SystemKeyword => "GiftCardPayment";

        public override IPayment CreatePayment(decimal amount, IOrderGroup orderGroup)
        {
            var payment = _orderGroupFactory.CreatePayment(orderGroup);
            payment.Properties.Add("GiftCardId", SelectedGiftCardId);
            payment.PaymentMethodId = PaymentMethodId;
            payment.PaymentMethodName = "GiftCardPayment";
            payment.Amount = amount;
            payment.Status = PaymentStatus.Pending.ToString();
            payment.TransactionType = TransactionType.Authorization.ToString();
            return payment;
        }

        public override bool ValidateData()
        {
            return true;
        }
    }
}