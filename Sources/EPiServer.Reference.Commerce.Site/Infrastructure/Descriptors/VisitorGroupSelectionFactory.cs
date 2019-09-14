using System.Collections.Generic;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using ISelectionFactory = EPiServer.Shell.ObjectEditing.ISelectionFactory;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    public class VisitorGroupSelectionFactory : ISelectionFactory
    {
        private Injected<IVisitorGroupRepository> visitorGroupRepo;

        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            List<SelectItem> items = new List<SelectItem>();

            foreach (var visitorGroup in visitorGroupRepo.Service.List())
            {
                items.Add(new SelectItem() { Text = visitorGroup.Name, Value = visitorGroup.Name });
            }

            return items;
        }
    }
}