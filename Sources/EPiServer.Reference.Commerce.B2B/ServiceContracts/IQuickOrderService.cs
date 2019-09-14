﻿using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;

namespace EPiServer.Reference.Commerce.B2B.ServiceContracts
{
    public interface IQuickOrderService
    {
        string ValidateProduct(ContentReference variationReference, decimal quantity, string code);
        ProductViewModel GetProductByCode(ContentReference productReference);
        decimal GetTotalInventoryByEntry(string code);
    }
}
