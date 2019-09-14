using System.Collections.Generic;
using System.Linq;
using EPiServer.Reference.Commerce.Shared.Constants;
using EPiServer.Reference.Commerce.Shared.Extensions;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Shared.EditorDescriptors
{
    public class CalloutContentAlignmentSelectionFactory : ISelectionFactory
    {
        public virtual IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var dic = new Dictionary<string, string>
            {
                { "None", CalloutContentAlignments.None },
                { "Left", CalloutContentAlignments.Left },
                { "Right", CalloutContentAlignments.Right },
                { "Center", CalloutContentAlignments.Center }
            };

            return dic.Select(x => new SelectItem { Text = x.Key, Value = x.Value });
        }
    }
}