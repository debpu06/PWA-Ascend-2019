using EPiServer.SpecializedProperties;

namespace EPiServer.Reference.Commerce.B2B.ServiceContracts
{
    public interface IB2BNavigationService
    {
        LinkItemCollection FilterB2BNavigationForCurrentUser(LinkItemCollection b2bLinks);
    }
}
