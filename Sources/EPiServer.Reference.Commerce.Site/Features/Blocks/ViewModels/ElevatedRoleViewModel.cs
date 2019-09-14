using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class ElevatedRoleViewModel : IBlockViewModel<ElevatedRoleBlock>
    {
        public ElevatedRoleViewModel(ElevatedRoleBlock block)
        {
            CurrentBlock = block;
            IsAccess = false;
        }
        public ElevatedRoleBlock CurrentBlock { get; }
        public bool IsAccess { get; set; }
    }
}
