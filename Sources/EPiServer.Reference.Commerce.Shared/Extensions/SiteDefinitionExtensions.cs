using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Shared.Extensions
{
    public static class SiteDefinitionExtensions
    {
        public static bool IsMosey(this SiteDefinition definition) => definition.Name == "Fabrikam" || definition.Name == "Mosey";
    }
}