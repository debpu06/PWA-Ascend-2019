using EPiServer.Reference.Commerce.Site.Features.Social.Composites;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Social.Comments.Core;
using EPiServer.Social.Common;
using EPiServer.Social.Ratings.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Services
{
    /// <summary>
    /// The ViewModelAdapter class converts Episerver Social models into
    /// corresponding view models, suitable for display in this application's
    /// user interface.
    /// </summary>
    internal static class ViewModelAdapter
    {
        /// <summary>
        /// Adapts Episerver Social's RatingStatistics model to a corresponding
        /// ReviewStatisticsViewModel.
        /// </summary>
        /// <param name="statistics">RatingStatistics to be converted</param>
        /// <returns>A corresponding ReviewStatisticsViewModel</returns>
        public static ReviewStatisticsViewModel Adapt(RatingStatistics statistics)
        {
            var viewModel = new ReviewStatisticsViewModel();
            
            if (statistics != null)
            {
                viewModel.OverallRating = Convert.ToDouble(statistics.Sum) / Convert.ToDouble(statistics.TotalCount);
                viewModel.TotalRatings = statistics.TotalCount;
                viewModel.Code = statistics.Target.Id.Replace("product://", "");
            }

            return viewModel;
        }

        /// <summary>
        /// Adapts a collection of composite comments (reviews) to a corresponding
        /// collection of ReviewViewModel.
        /// </summary>
        /// <param name="statistics">Collection of composites to be converted</param>
        /// <returns>A corresponding collection of ReviewViewModel</returns>
        public static IEnumerable<ReviewViewModel> Adapt(IEnumerable<Composite<Comment, Review>> reviews)
        {
            return reviews.Select(Adapt);
        }

        /// <summary>
        /// Adapts a composite comment (review) to a corresponding ReviewViewModel.
        /// </summary>
        /// <param name="review">Composite to be converted</param>
        /// <returns>A corresponding ReviewViewModel</returns>
        public static ReviewViewModel Adapt(Composite<Comment, Review> review)
        {
            return new ReviewViewModel
            {
                AddedOnStr = review.Data.Created.ToString("MM/dd/yyyy hh:mm:ss"),
                AddedOn = review.Data.Created,
                Body = review.Data.Body,
                Location = review.Extension.Location,
                Nickname = review.Extension.Nickname,
                Rating = review.Extension.Rating.Value,
                Title = review.Extension.Title,
                Id = review.Data.Id,
                Parent = review.Data.Parent,
                Author = review.Data.Author,
                IsVisible = review.Data.IsVisible
            };
        }
    }
}
