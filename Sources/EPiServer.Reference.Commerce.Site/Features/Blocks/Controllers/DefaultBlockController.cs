using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Web.Mvc;
using System;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true, Inherited = true)]
    public class DefaultBlockController : BlockController<SiteBlockData>
    {
        public override ActionResult Index(SiteBlockData currentBlock)
        {
            var model = CreateModel(currentBlock);
            return PartialView(string.Format("~/Views/Blocks/{0}.cshtml", currentBlock.GetOriginalType().Name), model);
        }

        private static IBlockViewModel<BlockData> CreateModel(BlockData currentBlock)
        {
            var type = typeof(BlockViewModel<>).MakeGenericType(currentBlock.GetOriginalType());
            return Activator.CreateInstance(type, currentBlock) as IBlockViewModel<BlockData>;
        }
    }
}