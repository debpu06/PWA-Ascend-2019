using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services;
using EPiServer.ServiceLocation;
using EPiServer.Social.Comments.Core;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommentManager.Services
{
    [ServiceConfiguration(typeof(ICommentManagerService))]
    public class CommentManagerService : ICommentManagerService
    {
        private readonly ICommentService _commentService;
        private readonly ContentLoaderService _contentLoaderService;
        private readonly IReviewService _reviewService;

        public CommentManagerService(ICommentService commentService, ContentLoaderService contentLoaderService, IReviewService reviewService)
        {
            _commentService = commentService;
            _contentLoaderService = contentLoaderService;
            _reviewService = reviewService;
        }

        public Comment Approve(string id)
        {
            var commentId = CommentId.Create(id);
            var comment = _commentService.Get(commentId);
            var updatedComment = new Comment(comment.Id, comment.Parent, comment.Author, comment.Body, true);
            return _commentService.Update(updatedComment);
        }

        public void Delete(string id)
        {
            var commentId = CommentId.Create(id);
            _commentService.Remove(commentId);
        }

        public IEnumerable<ReviewViewModel> Get(int page, int limit, out long total)
        {
            return _reviewService.Get(Visibility.All, page, limit, out total);
        }
    }
}