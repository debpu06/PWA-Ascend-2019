using EPiServer.Core;
using EPiServer.DataAbstraction;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    public abstract class SiteBlockData : BlockData
    {
        [Display(Name = "Padding Top", GroupName = "Block Padding", Order = 1)]
        public virtual int PaddingTop { get; set; }

        [Display(Name = "Padding Right", GroupName = "Block Padding", Order = 2)]
        public virtual int PaddingRight { get; set; }

        [Display(Name = "Padding Bottom", GroupName = "Block Padding", Order = 3)]
        public virtual int PaddingBottom { get; set; }

        [Display(Name = "Padding Left", GroupName = "Block Padding", Order = 4)]
        public virtual int PaddingLeft { get; set; }

        public string PaddingStyles
        {
            get
            {
                string paddingStyles = "";

                paddingStyles += (PaddingTop > 0) ? "padding-top: " + PaddingTop + "px;" : "";
                paddingStyles += (PaddingRight > 0) ? "padding-right: " + PaddingRight + "px;" : "";
                paddingStyles += (PaddingBottom > 0) ? "padding-bottom: " + PaddingBottom + "px;" : "";
                paddingStyles += (PaddingLeft > 0) ? "padding-left: " + PaddingLeft + "px;" : "";

                return paddingStyles;
            }
        }

        public override void SetDefaultValues(ContentType contentType)
        {            
            base.SetDefaultValues(contentType);

            PaddingTop = 0;
            PaddingRight = 0;
            PaddingBottom = 0;
            PaddingLeft = 0;
        }
    }
}