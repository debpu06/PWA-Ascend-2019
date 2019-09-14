using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Facets
{
    public class FacetRegistry : IFacetRegistry
    {
        private readonly List<FacetDefinition> _facetDefinitions;

        public FacetRegistry(IEnumerable<FacetDefinition> facetDefinitions)
        {
            _facetDefinitions = facetDefinitions.ToList();
        }

        public List<FacetDefinition> GetFacetDefinitions()
        {
            return _facetDefinitions;
        }

        public void AddFacetDefinitions(FacetDefinition facetDefinition)
        {
            _facetDefinitions.Add(facetDefinition);
        }

        public bool RemoveFacetDefinitions(FacetDefinition facetDefinition)
        {
            return _facetDefinitions.Remove(facetDefinition);
        }
    }
}