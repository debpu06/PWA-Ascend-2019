namespace EPiServer.Reference.Commerce.Site.Features.CampaignMailId.Interfaces
{
    /// <summary>
    /// Wrapper around the standard Episerver Campaign connector configuration
    /// </summary>
    public interface ICampaignToken
    {
        string GetToken();
    }
}