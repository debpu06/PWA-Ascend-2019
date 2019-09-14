using EPiServer.Find.Api.Facets;

namespace EPiServer.Reference.Commerce.Site.Features.Facets
{
    public class MultiSelectTermCount : TermCount, ISelectable
    {
        public bool Selected { get; set; }
    }
}