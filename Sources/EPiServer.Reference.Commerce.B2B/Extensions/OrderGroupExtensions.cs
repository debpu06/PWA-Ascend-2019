using System;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.B2B.Extensions
{
    public static class OrderGroupExtensions
    {
        #region OrderGroup extensions
        public static bool IsQuoteCart(this OrderGroup orderGroup)
        {
            return (orderGroup is Cart) && orderGroup.GetParentOrderId() != 0;
        }

        public static int GetParentOrderId(this OrderGroup orderGroup)
        {
            return orderGroup.GetIntegerValue(Constants.Quote.ParentOrderGroupId);
        }

        public static int GetIntegerValue(this OrderGroup orderGroup, string fieldName)
        {
            return orderGroup.GetIntegerValue(fieldName, 0);
        }

        public static int GetIntegerValue(this OrderGroup orderGroup, string fieldName, int defaultValue)
        {
            if (orderGroup[fieldName] == null)
            {
                return defaultValue;
            }
            return int.TryParse(orderGroup[fieldName].ToString(), out int retVal) ? retVal : defaultValue;
        }

        public static string GetStringValue(this OrderGroup orderGroup, string fieldName)
        {
            return DefaultIfNull(orderGroup[fieldName], string.Empty);
        }
        #endregion

        #region ICart extensions
        public static bool IsQuoteCart(this ICart orderGroup)
        {
            return orderGroup.GetParentOrderId() != 0;
        }

        public static int GetParentOrderId(this ICart orderGroup)
        {
            return orderGroup.GetIntegerValue(Constants.Quote.ParentOrderGroupId);
        }

        public static int GetIntegerValue(this ICart orderGroup, string fieldName)
        {
            return orderGroup.GetIntegerValue(fieldName, 0);
        }

        public static int GetIntegerValue(this ICart orderGroup, string fieldName, int defaultValue)
        {
            if (orderGroup.Properties[fieldName] == null)
            {
                return defaultValue;
            }
            return int.TryParse(orderGroup.Properties[fieldName].ToString(), out int retVal) ? retVal : defaultValue;
        }
        #endregion

        private static T DefaultIfNull<T>(object val, T defaultValue)
        {
            return val == null || val == DBNull.Value ? defaultValue : (T)val;
        }
    }
}
