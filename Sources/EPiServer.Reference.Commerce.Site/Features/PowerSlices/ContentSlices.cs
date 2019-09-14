using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Campaign.Pages;
using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using PowerSlice;

namespace EPiServer.Reference.Commerce.Site.Features.PowerSlices
{
    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class PagesSlice : ContentSliceBase<StandardPage>
    {
        public override string Name
        {
            get { return "Landing pages"; }
        }

        public override int SortOrder
        {
            get { return 10; }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class CampaignsSlice : ContentSliceBase<CampaignPage>
    {
        public override string Name
        {
            get { return "Campaign pages"; }
        }

        public override int SortOrder
        {
            get { return 11; }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class BlogsSlice : ContentSliceBase<BlogItemPage>
    {
        public override string Name
        {
            get { return "Blogs"; }
        }

        public override int SortOrder
        {
            get { return 12; }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class BlocksSlice : ContentSliceBase<SiteBlockData>
    {
        public override string Name
        {
            get { return "Blocks"; }
        }

        public override int SortOrder
        {
            get { return 50; }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class MediaSlice : ContentSliceBase<MediaData>
    {
        public override string Name
        {
            get { return "Media"; }
        }

        public override int SortOrder
        {
            get { return 70; }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class ImagesSlice : ContentSliceBase<ImageData>
    {
        public override string Name
        {
            get { return "Images"; }
        }

        public override int SortOrder
        {
            get { return 71; }
        }
    }
}