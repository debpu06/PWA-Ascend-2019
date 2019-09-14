using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.ViewModels;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Controllers
{
    public class TagCloudBlockController : BlockController<TagCloudBlock>
    {
        private readonly IContentLoader _contentLoader;
        private readonly CategoryRepository _categoryRepository;
        private readonly BlogTagFactory _blogTagFactory;

        public TagCloudBlockController(IContentLoader contentLoader,
            CategoryRepository categoryRepository,
            BlogTagRepository blogTagRepository,
            BlogTagFactory blogTagFactory)
        {
            _contentLoader = contentLoader;
            _categoryRepository = categoryRepository;
            _blogTagFactory = blogTagFactory;
        }

        public override ActionResult Index(TagCloudBlock currentBlock)
        {
            var model = new TagCloudModel(currentBlock)
            {
                Tags = GetTags(currentBlock.BlogTagLinkPage)
            };
         
            return PartialView(model);
        }

        public IEnumerable<BlogItemPageModel.TagItem> GetTags(ContentReference startTagLink)
        {
            var tags = new List<BlogItemPageModel.TagItem>();
            foreach (var item in BlogTagRepository.Instance.LoadTags())
            {
                var cat = _categoryRepository.Get(item.TagName);
                var url = string.Empty;

                if (startTagLink != null)
                {
                    url = _blogTagFactory.GetTagUrl(_contentLoader.Get<PageData>(startTagLink.ToPageReference()), cat);
                }

                tags.Add(new BlogItemPageModel.TagItem() { Count = item.Count, Title = item.DisplayName, Weight = item.Weight, Url = url });
            }
            return tags;
        }

    }
}
