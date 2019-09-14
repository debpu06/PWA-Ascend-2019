using EPiServer.DataAnnotations;
using System;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageScaleDescriptorAttribute : ImageDescriptorAttribute
    {
        public ImageScaleType ScaleMethod { get; set; }

        public ImageScaleDescriptorAttribute() : this(48, 48, ImageScaleType.ScaleToFill)
        {
        }

        public ImageScaleDescriptorAttribute(int width, int height, ImageScaleType scaleMethod)
        {
            Height = height;
            Width = width;
            ScaleMethod = scaleMethod;
        }
    }

    public enum ImageScaleType
    {
        ScaleToFit,
        ScaleToFill,
        Resize
    }
}