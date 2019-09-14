using EPiServer.Commerce.Marketing;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using PowerSlice;

namespace EPiServer.Reference.Commerce.Site.Features.PowerSlices
{
    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class FashionProductSlice : ContentSliceBase<BaseProduct>
    {
        public override string Name => "Products";

        public override int SortOrder => 100;
    }

    // Not all discounts show up - commenting until I find out why
    //[ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    //public class OrderPromotionSlice : ContentSliceBase<OrderPromotion>
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Order discounts";
    //        }
    //    }

    //    public override int SortOrder
    //    {
    //        get { return 111; }
    //    }
    //}

    //[ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    //public class ShippingSlice : ContentSliceBase<ShippingPromotion>
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Shipping discounts";
    //        }
    //    }

    //    public override int SortOrder
    //    {
    //        get { return 112; }
    //    }
    //}

    //[ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    //public class EntryPromotionSlice : ContentSliceBase<EntryPromotion>
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "Item discounts";
    //        }
    //    }

    //    public override int SortOrder
    //    {
    //        get { return 113; }
    //    }
    //}
}