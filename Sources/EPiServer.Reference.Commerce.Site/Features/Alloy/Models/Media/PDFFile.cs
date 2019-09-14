using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media
{
    [ContentType(GUID="58341C80-E78F-4F83-AF11-3B48563B41CA")]
    [MediaDescriptor(ExtensionString = "pdf")]
    public class PDFFile : MediaData, IFileProperties
    {
        ///// <summary>
        ///// Gets or sets the description.
        ///// </summary>
        //public virtual String Title { get; set; }

        [Editable(false)]
        public virtual string FileSize { get; set; }

        [Display(Name = "Allow Saving & Printing")]
        public virtual bool AllowPrintingSaving { get; set; }

        public virtual int? Scale
        {
            get
            {
                return this.GetPropertyValue(p => p.Scale) ?? 115;
            }
            set
            {
                this.SetPropertyValue(p => p.Scale, value);
            }
        }
    }

}


