using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Blocks.Ratings
{
    /// <summary>
    /// The LikeButtonBlock class defines the configuration used for rendering like button views.
    /// </summary>
    [ContentType(DisplayName = "Like Button Block", 
                 GUID = "1dae01b7-72ad-4a9d-b543-82b0f5af7bbc", 
                 Description = "A Like Button block implementation using the Episerver Social Ratings feature.", GroupName = "Social")]
    [ImageUrl("~/content/icons/blocks/cms-icon-block-25.png")]
    public class LikeButtonBlock : BlockData
    {
    }
}