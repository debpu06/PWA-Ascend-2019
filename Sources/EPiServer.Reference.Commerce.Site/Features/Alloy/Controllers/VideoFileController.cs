using System;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    /// <summary>
    /// Controller for the video file.
    /// </summary>
    public class VideoFileController : PartialContentController<VideoFile>
    {
        private readonly UrlResolver _urlResolver;

        public VideoFileController(UrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// The index action for the video file. Creates the view model and renders the view.
        /// </summary>
        /// <param name="currentContent">The current video file.</param>
        public override ActionResult Index(VideoFile currentContent)
        {
            var model = new VideoViewModel
            {
                Url = _urlResolver.GetUrl(currentContent.ContentLink),
                PreviewImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage) ? String.Empty : _urlResolver.GetUrl(currentContent.PreviewImage),
            };

            return PartialView(model);
        }
    }
}
