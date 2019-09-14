using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Social.Blocks.Comments;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Exceptions;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.Comments;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Comments;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Common;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Controllers
{
    /// <summary>
    /// The CommentsBlockController handles the rendering of the comment block frontend view as well
    /// as the posting of new comments.
    /// </summary>
    public class CommentsBlockController : SocialBlockController<CommentsBlock>
    {
        private readonly IUserRepository userRepository;
        private readonly IPageCommentRepository commentRepository;
        private readonly IPageRepository pageRepository;
        private readonly ICommunityActivityRepository activityRepository;
        private const string MessageKey = "CommentBlock";
        private const string SubmitSuccessMessage = "Your comment was submitted successfully!";
        private const string BodyValidationErrorMessage = "Cannot add an empty comment.";
        private const string ErrorMessage = "Error";
        private const string SuccessMessage = "Success";

        /// <summary>
        /// Constructor
        /// </summary>
        public CommentsBlockController()
        {
            this.userRepository = ServiceLocator.Current.GetInstance<IUserRepository>();
            this.commentRepository = ServiceLocator.Current.GetInstance<IPageCommentRepository>();
            this.pageRepository = ServiceLocator.Current.GetInstance<IPageRepository>();
            this.activityRepository = ServiceLocator.Current.GetInstance<ICommunityActivityRepository>();
        }

        /// <summary>
        /// Render the comment block frontend view.
        /// </summary>
        /// <param name="currentBlock">The current frontend block instance.</param>
        /// <returns>The action's result.</returns>
        public override ActionResult Index(CommentsBlock currentBlock)
        {
            var pageReference = this.pageRouteHelper.PageLink;
            var pageId = this.pageRepository.GetPageId(pageReference);

            // Create a comments block view model to fill the frontend block view
            var blockViewModel = new CommentsBlockViewModel(currentBlock, pageReference)
            {
                Messages = RetrieveMessages(MessageKey)
            };

            // Try to get recent comments
            try
            {
                var socialComments = this.commentRepository.Get(
                    new PageCommentFilter
                    {
                        Target = pageId.ToString(),
                        PageSize = currentBlock.CommentsDisplayMax
                    }
                );

                blockViewModel.Comments = socialComments;
            }
            catch (SocialRepositoryException ex)
            {
                blockViewModel.Messages.Add(new MessageViewModel(ex.Message, ErrorMessage));
            }

            return PartialView("~/Features/Social/Views/CommentsBlock/Index.cshtml", blockViewModel);
        }

        /// <summary>
        /// Submit handles the submitting of new comments.  It accepts a comment form model,
        /// validates the form, stores the submitted comment, sends a new activity if configuration
        /// allows, and redirects back to the current page.
        /// </summary>
        /// <param name="formViewModel">The comment form being submitted.</param>
        /// <returns>The submit action result.</returns>
        [HttpPost]
        public ActionResult Submit(CommentFormViewModel formViewModel)
        {
            var errors = ValidateCommentForm(formViewModel);

            if (errors.Count() == 0)
            {
                var addedComment = AddComment(formViewModel);
                if (addedComment != null && formViewModel.SendActivity)
                {
                    AddCommentActivity(addedComment);
                }
            }
            else
            {
                // Flag the CommentBody model state with validation error
                AddMessage(MessageKey, new MessageViewModel(errors.First(), ErrorMessage));
            }

            return Redirect(UrlResolver.Current.GetUrl(formViewModel.CurrentPageLink));
        }

        /// <summary>
        /// Adds the comment in the CommentFormViewModel to the Episerver Social repository.
        /// </summary>
        /// <param name="formViewModel">The submitted comment form view model.</param>
        /// <returns>The added PageComment</returns>
        private PageComment AddComment(CommentFormViewModel formViewModel)
        {
            var newComment = this.AdaptCommentFormViewModelToSocialComment(formViewModel);
            PageComment addedComment = null;

            try
            {
                addedComment = this.commentRepository.Add(newComment);
                AddMessage(MessageKey, new MessageViewModel(SubmitSuccessMessage, SuccessMessage));
            }
            catch (SocialRepositoryException ex)
            {
                AddMessage(MessageKey, new MessageViewModel(ex.Message, ErrorMessage));
            }

            return addedComment;
        }

        /// <summary>
        /// Add an activity for the newly added comment.
        /// </summary>
        /// <param name="comment">The added comment.</param>
        private void AddCommentActivity(PageComment comment)
        {
            try
            {
                var commentActivity = new PageCommentActivity { Body = comment.Body };

                this.activityRepository.Add(comment.AuthorId, comment.Target, commentActivity);
            }
            catch (SocialRepositoryException ex)
            {
                AddMessage(MessageKey, new MessageViewModel(ex.Message, ErrorMessage));
            }
        }

        /// <summary>
        /// Adapts a CommentFormViewModel to a PageComment.
        /// </summary>
        /// <param name="formViewModel">The comment form view model.</param>
        /// <returns>PageComment</returns>
        private PageComment AdaptCommentFormViewModelToSocialComment(CommentFormViewModel formViewModel)
        {
            return new PageComment
            {
                Target = this.pageRepository.GetPageId(formViewModel.CurrentPageLink),
                Body = formViewModel.Body,
                AuthorId = this.userRepository.GetUserId(this.User)
            };
        }

        /// <summary>
        /// Validates the comment form.
        /// </summary>
        /// <param name="formViewModel">The comment form view model.</param>
        /// <returns>Returns a list of validation errors.</returns>
        private List<string> ValidateCommentForm(CommentFormViewModel formViewModel)
        {
            var errors = new List<string>();

            // Make sure the comment body has some text
            if (string.IsNullOrWhiteSpace(formViewModel.Body))
            {
                errors.Add(BodyValidationErrorMessage);
            }

            return errors;
        }
    }
}
