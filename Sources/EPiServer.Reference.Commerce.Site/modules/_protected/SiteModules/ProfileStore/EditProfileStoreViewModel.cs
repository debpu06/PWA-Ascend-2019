namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class EditProfileStoreViewModel
    {
        public ProfileStoreItems ProfileStoreItems { get; set; }
        public ProfileStoreModel ProfileStoreModel { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public ScopeItems ScopeItems { get; set; }
        public SegmentItems SegmentItems { get; set; }
        public BlacklistItems BlacklistItems { get; set; }
        public TrackEventItems TrackEventItems { get; set; }
    }
}