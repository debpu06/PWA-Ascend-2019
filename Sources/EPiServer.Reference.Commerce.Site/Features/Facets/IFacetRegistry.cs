using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Facets
{
    public interface IFacetRegistry
    {
        List<FacetDefinition> GetFacetDefinitions();
        void AddFacetDefinitions(FacetDefinition facetDefinition);
        bool RemoveFacetDefinitions(FacetDefinition facetDefinition);
    }
}