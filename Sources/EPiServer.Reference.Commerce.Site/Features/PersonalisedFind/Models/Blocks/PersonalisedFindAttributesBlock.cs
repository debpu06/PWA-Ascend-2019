using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Models.Blocks
{
    [ContentType(GUID = "A2E1DFD3-4EB7-48CF-9CCE-328FEDCA864E", DisplayName = "Personalised Find Attributes", Description = "Used to expose attributes that are used when using Personalised Find. Useful when showing Personalised Find to technical audiences", GroupName = "Find")]
    [ImageUrl("~/episerver/Find/Resources/CustomizedSearchBlock.png")]
    public class PersonalisedFindAttributesBlock : BlockData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        public virtual string ShowAttributesText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [CultureSpecific]
        public virtual string ShowAttributesButtonClass { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 3)]
        [CultureSpecific]
        public virtual string HideAttributesText { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 4)]
        [CultureSpecific]
        public virtual string HideAttributesButtonClass { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 5)]
        [CultureSpecific]
        public virtual string ListHeading { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 6)]
        [CultureSpecific]
        public virtual string AttributeNameHeading { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 7)]
        [CultureSpecific]
        public virtual string AttributeValueHeading { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 8)]
        [CultureSpecific]
        public virtual string AttributeBoostFactorHeading { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            ShowAttributesText = "Red Pill";
            ShowAttributesButtonClass = "btn-danger";
            HideAttributesText = "Blue Pill";
            HideAttributesButtonClass = "btn-info";
            ListHeading = "Current attributes from Episerver Personalisation";
            AttributeNameHeading = "Attribute name";
            AttributeValueHeading = "Attribute value";
            AttributeBoostFactorHeading = "Boost factor";
        }
    }
}