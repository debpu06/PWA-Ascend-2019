namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public static class Constants
    {
        public static readonly string LoginPath = "/util/login.aspx";
        public static readonly string AppRelativeLoginPath = string.Format("~{0}", LoginPath);
        
        /// <summary>
        /// Tags to use for the main widths used in the Bootstrap HTML framework
        /// </summary>
        public static class ContentAreaTags
        {
           public const string NoRenderer = "norenderer";
        }

        /// <summary>
        /// Names used for UIHint attributes to map specific rendering controls to page properties
        /// </summary>
        public static class SiteUiHints
        {
            public const string Contact = "contact";
            public const string Strings = "StringList";
            public const string Parking = "parking";
            public const string Widget = "rec-widget";
        }

        /// <summary>
        /// Virtual path to folder with static graphics, such as "~/Static/gfx/"
        /// </summary>
        public const string StaticGraphicsFolderPath = "~/content/gfx/";


        /// <summary>
        /// Path to layout template for multisite
        /// </summary>
        public static class MasterViewSite
        {
            public const string AlloySite = "~/Features/Alloy/Views/Shared/Layouts/_Root.cshtml";
            public const string MoseySite = "~/Views/Shared/_LayoutMosey.cshtml";
            public const string MoseySupplySite = "~/Features/MoseySupply/Views/Shared/Layout/_Root.cshtml";
        }        

        /// <summary>
        /// Category names
        /// </summary>
        public static class CategoryName
        {
            public static string Fashion = "Fashion";
            public static string MoseySupply = "Mosey Supply";
        }
    }
}

