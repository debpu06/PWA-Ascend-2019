using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.ServiceLocation;
using System;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class ContentAreaItemExtensions
    {
        private static readonly Lazy<IContentLoader> ContentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());

        public static IBlockViewModel<T> GetBlockViewModel<T>(this ContentAreaItem contentAreaItem) where T : BlockData
        {
            if (contentAreaItem == null)
            {
                return null;
            }

            var block = ContentLoader.Value.Get<IContentData>(contentAreaItem.ContentLink) as T;

            return block != null ? CreateModel(block) : null;
        }

        private static IBlockViewModel<T> CreateModel<T>(T currentBlock) where T : BlockData
        {
            var type = typeof(BlockViewModel<>).MakeGenericType(currentBlock.GetOriginalType());
            return Activator.CreateInstance(type, currentBlock) as IBlockViewModel<T>;
        }
    }
}