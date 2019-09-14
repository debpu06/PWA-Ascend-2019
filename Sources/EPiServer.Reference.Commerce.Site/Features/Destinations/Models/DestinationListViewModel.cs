using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public class DestinationListViewModel
    {
        public string Heading { get; set; }
        public IEnumerable<DestinationPage> Destinations { get; set; }
        public ContentReference DestinationSection { get; set; }
        public bool ShowDescription { get; set; }
        public int Id { get; set; }
    }
}