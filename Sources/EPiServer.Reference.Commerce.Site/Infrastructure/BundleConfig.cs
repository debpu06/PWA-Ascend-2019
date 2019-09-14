using System.Web.Optimization;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Base
            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                "~/Assets/Base/jquery/jquery.min.js",
                "~/Assets/Base/bootstrap/bootstrap.min.js",
                "~/Assets/Base/jquery/jquery.easyautocomplete.min.js",
                "~/Assets/Base/slick/slick.min.js",
                "~/Assets/Base/Scripts/parallax.js",
                "~/Assets/Base/Scripts/revslider.js",
                "~/Assets/Base/Scripts/common.js",
                "~/Assets/Base/Scripts/jquery.bxslider.min.js",
                "~/Assets/Base/Scripts/owl.carousel.min.js",
                "~/Assets/Base/Scripts/jquery.mobile-menu.min.js",
                "~/Assets/Base/Scripts/cloud-zoom.js",
                "~/Assets/Base/Scripts/jquery.flexslider.js",
                "~/Assets/Base/Scripts/Chart.min.js",
                "~/Assets/Base/Scripts/popper.min.js",
                "~/Assets/Base/Scripts/d3.min.js",
                "~/Assets/Base/Scripts/topojson.min.js",
                "~/Assets/Base/Scripts/datamaps.world.min.js",
                "~/Scripts/jquery.signalR-2.4.1.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/base/jqueryExt").Include(
                "~/Assets/Base/lightbox/js/jquery.lightbox-0.5.js",
                "~/Assets/Base/slick/slick.min.js",
                "~/Assets/Base/jquery/jquery.easyautocomplete.min.js",
                "~/Assets/Base/jquery/jquery.validate.min.js",
                "~/Assets/Base/jquery/jquery.validate.unobtrusive.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/base/features").Include(
                "~/Assets/Base/Scripts/Features/*.js",
                "~/Assets/Base/Scripts/Features/Admin/*.js",
                "~/Assets/Base/Scripts/Features/B2b/*.js",
                "~/Assets/Base/Scripts/Features/moment.min.js",
                "~/Assets/Base/Scripts/Features/pickaday.min.js",
                "~/Assets/Base/Scripts/script.js"));

            bundles.Add(new StyleBundle("~/bundles/baseStyle").Include(
                "~/Assets/Base/font-awesome/css/font-awesome.css",
                "~/Assets/Base/lightbox/css/jquery.lightbox-0.5.css",
                "~/Assets/Base/TinyMceFontAwesome/fontawesome.min.css"
                ));

            // Alloy
            bundles.Add(new StyleBundle("~/bundlesAlloy/style").Include(
                "~/Assets/Alloy/alloy-style.css"));

            // Mosey
            bundles.Add(new StyleBundle("~/bundlesMosey/style").Include(
                "~/Assets/Mosey/Styles/mosey-style.css"));

           // bundles.Add(new ScriptBundle("~/bundlesMosey/js").Include(
           //     "~/Assets/Base/jquery/jquery.easyautocomplete.min.js",
           //     "~/Assets/Base/slick/slick.min.js",
           //     "~/Assets/Base/Scripts/parallax.js",
           //     "~/Assets/Base/Scripts/revslider.js",
           //     "~/Assets/Base/Scripts/common.js",
           //     "~/Assets/Base/Scripts/jquery.bxslider.min.js",
           //     "~/Assets/Base/Scripts/owl.carousel.min.js",
           //     "~/Assets/Base/Scripts/jquery.mobile-menu.min.js",
           //     "~/Assets/Base/Scripts/cloud-zoom.js",
           //     "~/Assets/Base/Scripts/jquery.flexslider.js", 
           //     "~/Assets/Base/Scripts/Chart.min.js",
           //     "~/Assets/Base/Scripts/popper.min.js",
           //     "~/Assets/Base/Scripts/d3.min.js",
           //     "~/Assets/Base/Scripts/topojson.min.js",
           //     "~/Assets/Base/Scripts/datamaps.world.min.js"));

           //bundles.Add(new ScriptBundle("~/bundlesMosey/features").Include(
           //     "~/Assets/Mosey/Scripts/Features/*.js",
           //     "~/Assets/Mosey/Scripts/Features/Admin/*.js",
           //     "~/Assets/Mosey/Scripts/Features/B2b/*.js",
           //     "~/Assets/Mosey/Scripts/Features/moment.min.js",
           //     "~/Assets/Mosey/Scripts/Features/pickaday.min.js",
           //     "~/Assets/Mosey/Scripts/script.js"));

            // Mosey Supply
            bundles.Add(new StyleBundle("~/bundlesMoseySupply/style").Include(
                "~/Assets/MoseySupply/Styles/moseysupply-style.css"
                ));

            //bundles.Add(new ScriptBundle("~/bundlesMoseySupply/js").Include(
            //    "~/Assets/Base/Scripts/revslider.js",
            //    "~/Assets/Base/Scripts/common.js",
            //    "~/Assets/Base/Scripts/jquery.bxslider.min.js",
            //    "~/Assets/Base/Scripts/owl.carousel.min.js",
            //    "~/Assets/Base/Scripts/jquery.mobile-menu.min.js",
            //    "~/Assets/Base/Scripts/cloud-zoom.js",
            //    "~/Assets/Base/Scripts/jquery.flexslider.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundlesMoseySupply/features").Include(
            //     "~/Assets/MoseySupply/Scripts/Features/Admin/*.js",
            //     "~/Assets/MoseySupply/Scripts/Features/*.js",
            //     "~/Assets/MoseySupply/Scripts/Features/B2b/*.js",
            //     "~/Assets/MoseySupply/Scripts/Features/moment.min.js",
            //     "~/Assets/MoseySupply/Scripts/Features/pickaday.min.js"));
        }
    }
}