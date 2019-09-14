using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = Constants.SiteUiHints.Widget)]
    public class RecommendationWidgetSelector : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            SelectionFactoryType = typeof(WidgetSelectionFactory);

            ClientEditingClass = "epi-cms/contentediting/editors/SelectionEditor";

            base.ModifyMetadata(metadata, attributes);
        }
    }
}