using EPiServer.Reference.Commerce.Shared.Constants;
using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Shared.EditorDescriptors
{
    public class TeaserColorThemeSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var dic = new Dictionary<string, string>
            {
                { "None", ColorThemes.None },
                { "Light", ColorThemes.Light },
                { "Dark", ColorThemes.Dark }
            };

            return dic.Select(x => new SelectItem { Text = x.Key, Value = x.Value });
        }
    }
}