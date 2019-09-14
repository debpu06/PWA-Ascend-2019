using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public interface IBlockViewModel<out T> 
        where T : BlockData
    {
        T CurrentBlock { get; }
    }
}