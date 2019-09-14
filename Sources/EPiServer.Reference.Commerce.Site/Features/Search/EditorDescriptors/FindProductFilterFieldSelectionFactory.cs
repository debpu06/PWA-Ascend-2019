using EPiServer.Find;
using EPiServer.Shell.ObjectEditing;
using Mediachase.MetaDataPlus.Configurator;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors
{
    public class FindProductFilterFieldSelectionFactory : ISelectionFactory
    {
        public virtual IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var list = MetaField.GetList(new Mediachase.MetaDataPlus.MetaDataContext()).Where(x => x.Namespace.Equals("Mediachase.Commerce.Catalog"));
            return list.Select(x => new SelectItem() { Text = x.FriendlyName, Value = x.Name });
        }
    }
}