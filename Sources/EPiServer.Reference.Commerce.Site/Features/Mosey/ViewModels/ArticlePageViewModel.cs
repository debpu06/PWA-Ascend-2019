using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Helper;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System;
using System.Linq;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Helper;

namespace EPiServer.Reference.Commerce.Site.Features.Mosey.ViewModels
{
    public class ArticlePageViewModel : ContentViewModel<ArticlePage>
    {
        public ArticlePageViewModel(ArticlePage articlePage, CategoryRepository categoryRepository) : base(articlePage)
        {
            string cssClass;
            switch (CurrentContent.TopPaddingMode)
            {

                case MoseyArticlePageTopPaddingModeSelectionFactory.MoseyArticlePageTopPaddingModes.Half:
                    cssClass = "hero-image-bg-half";
                    break;
                case MoseyArticlePageTopPaddingModeSelectionFactory.MoseyArticlePageTopPaddingModes.Full:
                    cssClass = "hero-image-bg-full";
                    break;
                case MoseyArticlePageTopPaddingModeSelectionFactory.MoseyArticlePageTopPaddingModes.None:
                    cssClass = "hero-image-bg-none";
                    break;
                default:
                    cssClass = null;
                    break;
            }

            TopPaddingCssClass = cssClass;

            string categoryDisplayName;
            if (articlePage.Category != null && articlePage.Category.Any())
            {
                categoryDisplayName  = categoryRepository.Get(articlePage.Category.FirstOrDefault()).Description;
            }
            else
            {
                categoryDisplayName = null;
            }

            CategoryName = categoryDisplayName;

        }
        public string CategoryName { get; }
        public string TopPaddingCssClass { get; }
    }
}