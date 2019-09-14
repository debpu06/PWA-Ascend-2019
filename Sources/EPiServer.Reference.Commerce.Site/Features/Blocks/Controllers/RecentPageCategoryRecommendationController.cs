using EPiServer.DataAbstraction;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Globalization;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true)]
    public class RecentPageCategoryRecommendationController : BlockController<RecentPageCategoryRecommendation>
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly IClient _findClient;
        private readonly Random _random = new Random();
        private readonly IContentLoader _contentLoader;

        public RecentPageCategoryRecommendationController(CategoryRepository categoryRepository, 
            IClient findClient, 
            IContentLoader contentLoader)
        {
            _categoryRepository = categoryRepository;
            _findClient = findClient;
            _contentLoader = contentLoader;
        }

        public override ActionResult Index(RecentPageCategoryRecommendation currentBlock)
        {
            var categories = Shared.Extensions.ContentExtensions.GetPageBrowseHistory()
                .Reverse()
                .SelectMany(x => x.Category)
                .ToList();

            var pages = new List<ArticlePage>();

            if (categories.Any())
            {
                var pageResult = _findClient.Search<ArticlePage>()
                    .Filter(x => x.Language.Name.Match(ContentLanguage.PreferredCulture.Name))
                    .AddFilterForIntList(categories, "Category")
                    .GetContentResult();

                pages = pageResult.Items.ToList();
            }
           
            var model = new RecentPageCategoryRecommendationViewModel(currentBlock)
            {
                CategoryPages = new List<CategoryViewModel>()
            };

            if (!pages.Any())
            {
                if (ContentReference.IsNullOrEmpty(currentBlock.InspirationFolder))
                {
                    return PartialView("/Views/Blocks/RecentPageCategoryRecommendation.cshtml", model);
                }

                pages = _contentLoader.GetChildren<ArticlePage>(currentBlock.InspirationFolder)
                    .ToList();
            }

            var pageCount = pages.Count;
            var count = Math.Max(currentBlock.NumberOfRecommendations, pageCount);
            if (count > currentBlock.NumberOfRecommendations)
            {
                count = currentBlock.NumberOfRecommendations;
            }
            foreach (var page in PickSomeInRandomOrder(pages, count))
            {
                var categoryCount = page.Category.Count;
                var categoryInt = 0;

                if (categories.Any())
                {
                    categoryInt = categories[_random.Next(0, categories.Count - 1)];
                }

                if (categoryInt == 0)
                {
                    categoryInt = page.Category[_random.Next(0, categoryCount - 1)];
                }

                var category = _categoryRepository.Get(categoryInt);
                model.CategoryPages.Add(new CategoryViewModel
                {
                    Id = category.ID,
                    Name = category.Name,
                    Page = page
                });

            }
            return PartialView("/Views/Blocks/RecentPageCategoryRecommendation.cshtml", model);
        }

        private IEnumerable<T> PickSomeInRandomOrder<T>(IEnumerable<T> someTypes,
            int maxCount)
        {
            var randomSortTable = new Dictionary<double, T>();

            foreach (var someType in someTypes)
            {
                randomSortTable[_random.NextDouble()] = someType;
            }

            return randomSortTable.OrderBy(kvp => kvp.Key).Take(maxCount).Select(kvp => kvp.Value);
        }
    }
}
