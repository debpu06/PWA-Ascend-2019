using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.Mosey.Helper
{
    public class MoseyArticlePageTopPaddingModeSelectionFactory : ISelectionFactory
    {
        public static class MoseyArticlePageTopPaddingModes
        {
            public const string None = "None";
            public const string Half = "Half";
            public const string Full = "Full";
        }
        public virtual IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var dic = new Dictionary<string, string>
            {
                { "None", MoseyArticlePageTopPaddingModes.None },
                { "Half", MoseyArticlePageTopPaddingModes.Half },
                { "Full", MoseyArticlePageTopPaddingModes.Full },
            };

            return dic.Select(x => new SelectItem { Text = x.Key, Value = x.Value });
        }
    }
}