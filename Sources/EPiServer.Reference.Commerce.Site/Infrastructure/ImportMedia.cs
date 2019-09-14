using EPiServer.Commerce.Internal.Migration.Steps;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    [ServiceConfiguration(typeof(IMigrationStep))]
    public class ImportMedia : IMigrationStep
    {
        public int Order => 1200;
        public string Name => "Quicksilver Media";
        public string Description => "Import Media";

        public bool Execute(IProgressMessenger progressMessenger)
        {
            return true;
        }

        
    }
}