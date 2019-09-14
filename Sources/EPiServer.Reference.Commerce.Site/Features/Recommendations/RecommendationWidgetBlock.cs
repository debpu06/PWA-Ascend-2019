using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.Recommendations
{
    [ContentType(DisplayName = "Recommendation Widget",
        GUID = "d5cc427b-afa4-4c4d-8986-eb5f73e0b9fe",
        Description = "Block that adds recommendations based on selcted widget type.",
        GroupName = "Personalization")]
    [ImageUrl("~/Content/icons/blocks/CMS-icon-block-07.png")]
    public class RecommendationWidgetBlock : BlockData
    {

        [SelectOne(SelectionFactoryType = typeof(WidgetSelectionFactory))]
        [Display(
            Name = "Widget Type",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string WidgetType { get; set; }

        [Display(
            Name = "Attribute Name",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual string Name { get; set; }

        [Display(
            Name = "Attribute Value",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual string Value { get; set; }

        [Display(
            Name = "Number Of Recommendations",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual int NumberOfRecommendations { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            NumberOfRecommendations = 4;
        }

    }
}