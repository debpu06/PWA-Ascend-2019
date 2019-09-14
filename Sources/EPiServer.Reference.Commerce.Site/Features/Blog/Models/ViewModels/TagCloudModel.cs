using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.ViewModels
{
    public class TagCloudModel : BlockViewModel<TagCloudBlock>
    {
        public TagCloudModel(TagCloudBlock block) : base(block)
        {
            Heading = block.Heading;    
        }

        public string Heading { get; set; }

        public IEnumerable<BlogItemPageModel.TagItem> Tags { get; set; }

    }
}