using EPiServer.Find;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using System.Collections.Specialized;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public interface IFilterBlock
    {
        string FilterTitle { get; set; }

        ITypeSearch<DestinationPage> AddFilter(ITypeSearch<DestinationPage> query);

        ITypeSearch<DestinationPage> ApplyFilter(ITypeSearch<DestinationPage> query, NameValueCollection filters );
    }
}
