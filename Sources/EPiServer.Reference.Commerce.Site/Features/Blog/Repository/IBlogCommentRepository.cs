using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Comment;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.Comments;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Repository
{
    /// <summary>
    /// The IBlogCommentRepository interface defines the operations that can be issued
    /// against a blog comment repository.
    /// </summary>
    public interface IBlogCommentRepository
    {
        /// <summary>
        /// Adds a comment to the underlying comment repository.
        /// </summary>
        /// <param name="comment">The comment to add.</param>
        /// <returns>The added comment.</returns>
        BlogComment Add(BlogComment comment);

        /// <summary>
        /// Gets comments from the underlying comment repository based on a filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>A list of comments.</returns>
        IEnumerable<BlogComment> Get(PageCommentFilter filter, out long total);
    }

}