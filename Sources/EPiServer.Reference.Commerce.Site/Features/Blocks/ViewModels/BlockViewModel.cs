using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class BlockViewModel<T> : IBlockViewModel<T>
        where T : BlockData
    {
        public BlockViewModel(T currentBlock)
        {
            CurrentBlock = currentBlock;
        }

        public T CurrentBlock { get; private set; }
    }
}