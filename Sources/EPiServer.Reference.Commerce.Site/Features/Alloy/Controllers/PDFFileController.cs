using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    [TemplateDescriptor(TemplateTypeCategory = TemplateTypeCategories.MvcPartialController, Default = true, ModelType = typeof(PDFFile))]
    public class PDFFileController : PartialContentController<PDFFile>
    {
        private IUrlResolver _urlResolver;
        public PDFFileController(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public override ActionResult Index(PDFFile currentContent)
        {
            string friendlyUrl = UrlResolver.Current.GetUrl(
                currentContent.ContentLink,
                null,
                new VirtualPathArguments { ContextMode = ContextMode.Default });

            ViewBag.FriendlyUrl = friendlyUrl;
            return PartialView(currentContent);
        }
    }
}
