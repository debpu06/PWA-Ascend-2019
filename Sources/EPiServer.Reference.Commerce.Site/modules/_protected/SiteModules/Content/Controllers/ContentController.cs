using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Controllers
{
    public class ContentController : Controller
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [MenuItem("/global/commerce/bulkupdate", TextResourceKey = "/Shared/Bulk")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetContentTypes(string type)
        {
            var contentTypes = _contentService.GetContentTypes(type);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(contentTypes),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public ActionResult GetProperties(int id)
        {

            var properties = _contentService.GetProperties(id);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(properties),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public ActionResult GetLanguages()
        {
            var languages = _contentService.GetLanguages();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(languages),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public ActionResult Get(int contentTypeId, string language, string properties, string keyword = "")
        {
            var contents = _contentService.GetByContentType(contentTypeId, language, properties, keyword);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(contents),
                ContentType = "application/json",
            };
        }

        [HttpPost]
        public ActionResult UpdateContent(UpdateContentModel model)
        {
            string message = "";
            foreach (var content in model.Contents)
            {
                _contentService.UpdateContent(content, model.Properties, out message);
            }

            return new ContentResult
            {
                Content = message
            };
        }
    }
}
