using EPiServer.Commerce.Marketing;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Blocks;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels
{
    public class AlmostDiscountViewModel : BlockViewModel<AlmostDiscountBlock>
    {
        public AlmostDiscountViewModel(AlmostDiscountBlock almostDiscountBlock) : base(almostDiscountBlock)
        {
            
        }

        public IEnumerable<RewardDescription> Rewards { get; set; }
        public Decimal Total { get; set; }

    }
}