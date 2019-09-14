using System;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Runtime.Remoting.Contexts;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Controllers
{
    public class BlogItemController : PageController<BlogItemPage>
    {
        private readonly BlogTagFactory _blogTagFactory;
        private readonly CategoryRepository _categoryRepository;

        public BlogItemController(BlogTagFactory blogTagFactory,
            CategoryRepository categoryRepository)
        {
            _blogTagFactory = blogTagFactory;
            _categoryRepository = categoryRepository;
        }

        public int PreviewTextLength { get; set; }

        public ActionResult Preview(PageData currentPage, BlogListModel blogModel)
        {
            var pd = (BlogItemPage)currentPage;
            PreviewTextLength = 200;

            var model = new BlogItemPageModel(pd)
            {
                Tags = GetTags(pd),
                PreviewText = GetPreviewText(pd),
                ShowIntroduction = blogModel.ShowIntroduction,
                ShowPublishDate = blogModel.ShowPublishDate,
                StartPublish = currentPage.StartPublish ?? DateTime.UtcNow
            };

            return PartialView("Preview", model);
        }

        public ActionResult Full(BlogItemPage currentPage)
        {
            PreviewTextLength = 200;

            var model = new BlogItemPageModel(currentPage)
            {
                Category = currentPage.Category,
                Tags = GetTags(currentPage),
                PreviewText = GetPreviewText(currentPage),
                MainBody = currentPage.MainBody,
                StartPublish = currentPage.StartPublish ?? DateTime.UtcNow
            };

            var editHints = ViewData.GetEditHints<BlogItemPageModel, BlogItemPage>();
            editHints.AddConnection(m => m.Category, p => p.Category);
            editHints.AddFullRefreshFor(p => p.Category);
            editHints.AddFullRefreshFor(p => p.StartPublish);

            return PartialView("Full", model);
        }

        public ActionResult Index(BlogItemPage currentPage)
        {

            PreviewTextLength = 200;

            var model = new BlogItemPageModel(currentPage)
            {
                Category = currentPage.Category,
                Tags = GetTags(currentPage),
                PreviewText = GetPreviewText(currentPage),
                MainBody = currentPage.MainBody,
                StartPublish = currentPage.StartPublish ?? DateTime.UtcNow
            };
            var editHints = ViewData.GetEditHints<ContentViewModel<BlogItemPage>, BlogItemPage>();
             editHints.AddConnection(m => m.CurrentContent.Category, p => p.Category);
             editHints.AddConnection(m => m.CurrentContent.StartPublish, p => p.StartPublish);


            return View(model);
        }

        public IEnumerable<BlogItemPageModel.TagItem> GetTags(BlogItemPage currentPage)
        {
            return currentPage.Category.Select(item => _categoryRepository.Get(item)).
                Select(cat => new BlogItemPageModel.TagItem()
                {
                    Title = cat.Name,
                    DisplayName = cat.Description,
                    Url = _blogTagFactory.GetTagUrl(currentPage, cat)
                }).ToList();
        }

        protected string GetPreviewText(BlogItemPage page)
        {
            if (PreviewTextLength <= 0)
            {
                return string.Empty;
            }

            var previewText = string.Empty;

            if (page.MainBody != null)
            {
                previewText = page.MainBody.ToHtmlString();
            }

            if (string.IsNullOrEmpty(previewText))
            {
                return string.Empty;
            }

            var regexPattern = new StringBuilder(@"<span[\s\W\w]*?classid=""");
            //regexPattern.Append(DynamicContentFactory.Instance.DynamicContentId.ToString());
            regexPattern.Append(@"""[\s\W\w]*?</span>");
            previewText = Regex.Replace(previewText, regexPattern.ToString(), string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return TextIndexer.StripHtml(previewText, PreviewTextLength);
        }


    }
}
