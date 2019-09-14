using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public abstract class CartViewModelBase<T> : ContentViewModel<T> where T : IContent
    {
        protected CartViewModelBase(T content) : base(content)
        {
            
        }
        public decimal ItemCount { get; set; }

        public IEnumerable<CartItemViewModel> CartItems { get; set; }

        public Money Total { get; set; }

        public bool HasOrganization { get; set; }
    }
}