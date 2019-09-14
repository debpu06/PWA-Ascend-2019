using EPiServer.Commerce.Order;
using Mediachase.BusinessFoundation.Data;
using Mediachase.BusinessFoundation.Data.Business;
using System;

namespace EPiServer.Reference.Commerce.GiftCardPayment
{
    public static class GiftCardManager
    {
        public const string GiftCardMetaClass = "GiftCard";
        public const string ContactMetaClass = "Contact";
        public const string GiftCardIdField = "GiftCardId";
        public const string GiftCardNameField = "GiftCardName";
        public const string ContactIdField = "ContactId";
        public const string InitialAmountField = "InitialAmount";
        public const string RemainBalanceField = "RemainBalance";
        public const string IsActiveField = "IsActive";
        public const string RedemptionCodeField = "RedemptionCode";
        
        public static EntityObject[] GetAllGiftCards()
        {
            return BusinessManager.List(GiftCardMetaClass, new FilterElement[0]);
        }

        public static EntityObject[] GetCustomerGiftCards(PrimaryKeyId contactId)
        {
            return BusinessManager.List(GiftCardMetaClass, new FilterElement[]
                {
                    FilterElement.EqualElement(ContactIdField, contactId)
                });
        }

        public static PrimaryKeyId CreateGiftCard(string giftCardName, PrimaryKeyId contactId, decimal initialAmount, decimal remainBalance, string redemptionCode, bool isActive)
        {
            EntityObject giftCard = BusinessManager.InitializeEntity(GiftCardMetaClass);
            giftCard[GiftCardNameField] = giftCardName;
            giftCard[ContactIdField] = contactId;
            giftCard[InitialAmountField] = initialAmount;
            giftCard[RemainBalanceField] = remainBalance;
            giftCard[IsActiveField] = isActive;
            giftCard[RedemptionCodeField] = redemptionCode;
            PrimaryKeyId giftCardId = BusinessManager.Create(giftCard);
            return giftCardId;
        }

        public static void UpdateGiftCard(PrimaryKeyId giftCardId, string giftCardName, PrimaryKeyId contactId, decimal initialAmount, decimal remainBalance, string redemptionCode, bool isActive)
        {
            EntityObject giftCard = BusinessManager.Load(GiftCardMetaClass, giftCardId);
            giftCard[GiftCardNameField] = giftCardName;
            giftCard[ContactIdField] = contactId;
            giftCard[InitialAmountField] = initialAmount;
            giftCard[RemainBalanceField] = remainBalance;
            giftCard[IsActiveField] = isActive;
            giftCard[RedemptionCodeField] = redemptionCode;
            BusinessManager.Update(giftCard);
        }

        public static void DeleteGiftCard(PrimaryKeyId giftCardId)
        {
            EntityObject giftCard = BusinessManager.Load(GiftCardMetaClass, giftCardId);
            BusinessManager.Delete(giftCard);
        }

        public static bool PurchaseByGiftCard(IPayment payment)
        {
            EntityObject giftCard = BusinessManager.Load(GiftCardMetaClass, new PrimaryKeyId(new Guid(payment.Properties["GiftCardId"].ToString())));
            if(payment.Amount > (decimal)giftCard[RemainBalanceField])
            {
                return false;
            }
            else
            {
                giftCard[RemainBalanceField] = (decimal)giftCard[RemainBalanceField] - payment.Amount;
                if((decimal)giftCard[RemainBalanceField] <= 0)
                {
                    giftCard[IsActiveField] = false;
                }
                BusinessManager.Update(giftCard);
                return true;
            }
        }
    }
}
