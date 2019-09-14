using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors
{
    public class NumericOperatorSelectionFactory : ISelectionFactory
    {
        public class OperatorNames
        {
            public const string Equal = "Equal";
            public const string GreaterThan = "GreaterThan";
            public const string LessThan = "LessThan";
        }

        public virtual IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var dic = new Dictionary<string, string>()
            {
                {"Equals", OperatorNames.Equal},
                {"Greater Than", OperatorNames.GreaterThan},
                {"Less Than", OperatorNames.LessThan}
            };

            return dic.Select(x => new SelectItem() { Text = x.Key, Value = x.Value });
        }
    }
}