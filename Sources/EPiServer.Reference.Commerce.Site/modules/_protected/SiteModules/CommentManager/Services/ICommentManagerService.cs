using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Social.Comments.Core;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommentManager.Services
{
    public interface ICommentManagerService
    {
        IEnumerable<ReviewViewModel> Get(int page, int limit, out long total);
        void Delete(string id);
        Comment Approve(string id);
    }
}
