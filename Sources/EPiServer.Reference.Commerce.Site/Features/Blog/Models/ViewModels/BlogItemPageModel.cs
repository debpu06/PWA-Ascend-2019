using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.ViewModels
{
    public class BlogItemPageModel : ContentViewModel<BlogItemPage>
    {
        public BlogItemPageModel(BlogItemPage currentPage)
            : base(currentPage)
        {}

        public IEnumerable<TagItem> Tags { get; set; }
     
        public string PreviewText { get; set; }
        public DateTime StartPublish { get; set; }
        public XhtmlString MainBody { get; set; }

        public bool ShowPublishDate { get; set; }

        public bool ShowIntroduction { get; set; }
        public CategoryList Category { get; set; }

        public class TagItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public int Weight { get; set; }
            public int Count { get; set; }
            public string DisplayName { get; set; }
        }

    }
}