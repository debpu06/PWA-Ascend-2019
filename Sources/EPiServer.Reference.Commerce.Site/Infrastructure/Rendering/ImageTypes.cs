﻿using ImageProcessor.Web.Episerver;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Rendering
{
    public static class ImageTypes
    {
        // A full width Hero image is very simple, since its always 100% of the viewport width.
        public static ImageType HeroImage = new ImageType
        {
            DefaultImgWidth = 1280,
            SrcSetWidths = new[] { 375, 750, 1440, 1920 },
            SrcSetSizes = new[] { "100vw" },
            HeightRatio = 0.5625 //16:9
        };

        //This thumbnail is always 200 px, but the browser may select the 400 px image for screens with device pixel ratio > 1.
        public static ImageType Thumbnail = new ImageType
        {
            DefaultImgWidth = 200,
            SrcSetWidths = new[] { 200, 400 },
            SrcSetSizes = new[] { "200px" },
            HeightRatio = 1 //square
        };

        // Up to 980 pixels viewport width, the image width will be 100% of the viewport - 40 pixels (margins).
        // Up to 1200 pixels viewport width, the image width will be 368 pixels.
        // On larger viewport widths, the image width will be 750 pixels.
        public static ImageType Teaser = new ImageType
        {
            DefaultImgWidth = 750,
            SrcSetWidths = new[] { 375, 750, 980, 1500 },
            SrcSetSizes = new[] { "(max-width: 980px) calc((100vw - 40px))", "(max-width: 1200px) 368px", "750px" }
        };
    }
}