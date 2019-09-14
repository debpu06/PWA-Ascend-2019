using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.ViewModels
{
    public class BlogListModel : BlockViewModel<BlogListBlock>
    {
        public BlogListModel(BlogListBlock block) : base(block)
        {
            Heading = block.Heading;
            ShowIntroduction = block.IncludeIntroduction;
            ShowPublishDate = block.IncludePublishDate;
        }
        public string Heading { get; set; }
        public IEnumerable<PageData> Blogs { get; set; }
        public bool ShowIntroduction { get; set; }
        public bool ShowPublishDate { get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}