using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using Mediachase.Commerce.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors
{
    public class FindCatalogSelectionFactory : ISelectionFactory
    {
        public virtual IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var _referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
            var catalogs = _contentRepository.GetChildren<CatalogContentBase>(_referenceConverter.GetRootLink());
            var items = catalogs.Select(x => new SelectItem() { Text = x.Name, Value = x.CatalogId }).ToList();
            items.Insert(0, new SelectItem() { Text = "All", Value = 0 });
            return items;
        }
    }
}