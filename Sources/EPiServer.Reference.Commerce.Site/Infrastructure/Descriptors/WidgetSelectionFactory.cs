using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    public class WidgetSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return new List<SelectItem>()
            {
                new SelectItem
                {
                    Text = "Home Page",
                    Value = "Home"
                },
                new SelectItem
                {
                    Text = "Cart Page",
                    Value = "Basket"
                },
                new SelectItem
                {
                    Text = "Checkout Page",
                    Value = "Checkout"
                },
                new SelectItem
                {
                    Text = "Search Results Page",
                    Value = "Search"
                },
                new SelectItem
                {
                    Text = "Wishlist Page",
                    Value = "Wishlist"
                },
                new SelectItem
                {
                    Text = "Order Confirmation Page",
                    Value = "Order"
                },
                new SelectItem
                {
                    Text = "Attribute Page",
                    Value = "Attribute"
                },
                new SelectItem
                {
                    Text = "Brand Page",
                    Value = "Brand"
                },
                new SelectItem
                {
                    Text = "Default Page",
                    Value = "Default"
                },
            };
        }
    }
}