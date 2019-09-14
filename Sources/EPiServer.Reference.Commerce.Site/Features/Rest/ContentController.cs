using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Web;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Rest
{
    [System.Web.Http.RoutePrefix("episerverapi")]
    public class ContentController : ApiController
    {
        private readonly ITemplateResolver _templateResolver;
        private readonly ContentAreaRenderer _contentAreaRenderer;
        private readonly IContentRenderer _contentRender;
        private readonly IContentRepository _contentRepository;
        private readonly PartialRequest _partialRequest;
        private readonly IContentVersionRepository _contentVersionRepository;

        public ContentController(ITemplateResolver templateResolver,
            ContentAreaRenderer contentAreaRenderer,
            IContentRepository contentRepository,
            PartialRequest partialRequest,
            IContentRenderer contentRender,
            IContentVersionRepository contentVersionRepository)
        {
            _templateResolver = templateResolver;
            _contentAreaRenderer = contentAreaRenderer;
            _contentRepository = contentRepository;
            _partialRequest = partialRequest;
            _contentRender = contentRender;
            _contentVersionRepository = contentVersionRepository;
        }

        [System.Web.Http.Route("content/{id}/{date:DateTime?}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetContent(string id, DateTime? date = null)
        {
            var contentLink = LookupRef(id);
            if (contentLink == ContentReference.EmptyReference)
            {
                return NotFound();
            }

            if (date.HasValue)
            {
                contentLink = GetVersionByDate(contentLink, date.Value);
            }

            var matchedContent = _contentRepository.Get<IContent>(contentLink);
            if (Request.Headers.Accept.Any(x => x.MediaType.Equals("text/html")))
            {
                return RenderContent(matchedContent);
            }
            return Ok(ConstructExpandoObject(matchedContent));
        }

        // GET api/<controller>
        public IHttpActionResult GetContentArea(string contentLink)
        {
            var matchedContent = _contentRepository.Get<IContent>(ContentReference.Parse(contentLink));
            return RenderContent(matchedContent);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetPage(string contentLink, string name)
        {
            var matchedContent = _contentRepository.Get<IContent>(ContentReference.Parse(contentLink));
            return RenderContentArea(matchedContent, name);
        }
        
        private ContentReference GetVersionByDate(ContentReference publishedReference, DateTime publishDate)
        {
            IEnumerable<ContentVersion> versions;
            VersionFilter filter;
            int matchedRows;

            if (DateTime.UtcNow >= publishDate)
            {
                filter = new VersionFilter
                {
                    Statuses = new[] { VersionStatus.PreviouslyPublished },
                    ContentLink = publishedReference
                };
                versions = _contentVersionRepository.List(filter, 0, Int32.MaxValue, out matchedRows)
                    .Where(v => v.Saved < publishDate)
                    .OrderBy(x => publishDate - x.Saved);

                return versions.Any() ? versions.First().ContentLink : publishedReference;
            }

            filter = new VersionFilter
            {
                Statuses = new[] { VersionStatus.DelayedPublish },
                ContentLink = publishedReference
            };
            versions = _contentVersionRepository.List(filter, 0, Int32.MaxValue, out matchedRows)
                .Where(v => v.DelayPublishUntil.HasValue && v.DelayPublishUntil < publishDate)
                .OrderBy(x => publishDate - x.DelayPublishUntil);

            return versions.Any() ? versions.First().ContentLink : publishedReference;
        }

        protected ContentReference LookupRef(string Ref)
        {
            switch (Ref.ToLower())
            {
                case "root":
                    return ContentReference.RootPage;
                case "start":
                    return ContentReference.StartPage;
                case "globalblock":
                    return ContentReference.GlobalBlockFolder;
                case "siteblock":
                    return ContentReference.SiteBlockFolder;
            }
            var c = ContentReference.EmptyReference;
            if (ContentReference.TryParse(Ref, out c))
            {
                return c;
            }
            var g = Guid.Empty;
            if (Guid.TryParse(Ref, out g))
            {
                PermanentLinkUtility.FindContentReference(g);
            }
            return ContentReference.EmptyReference;
        }

        protected ContentReference LookupRef(ContentReference parent, string contentType, string name)
        {
            var content = _contentRepository.GetChildren<IContent>(parent).FirstOrDefault(ch => ch.GetType().Name == contentType && ch.Name == name);
            return content == null ? ContentReference.EmptyReference : content.ContentLink;
        }

        private HtmlActionResult RenderContent(IContent content)
        {
            if (content == null)
            {
                throw new ContentNotFoundException("Content was not found");
            }

            //Resolve the right Template based on Episervers own templating engine
            var model = _templateResolver.Resolve(HttpContext.Current, content.GetOriginalType(), TemplateTypeCategories.Mvc | TemplateTypeCategories.MvcPartial, new string[0]);

            //Resolve the controller
            var contentController = ServiceLocator.Current.GetInstance(model.TemplateType) as ControllerBase;

            //Derive the name
            var controllerName = model.Name.Replace("Controller", "");

            //Mimic the routing of our rendition 
            var routeData = new RouteData();
            routeData.Values.Add("currentContent", content);
            routeData.Values.Add("controllerType", model.TemplateType);
            routeData.Values.Add("language", ContentLanguage.PreferredCulture.Name);
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("node", content.ContentLink.ID);

            var viewData = new ViewDataDictionary
            {
                { "HasContainer", false }
            };


            //Create a fake context, that can be executed based on the route
            var viewContext = new ViewContext(
                new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, contentController),
                new FakeView(),
                viewData,
                new TempDataDictionary(),
                new StringWriter());

            var helper = new HtmlHelper(viewContext, new ViewPage());

            //Render in our fake context
            _contentRender.Render(helper, _partialRequest, content, model);

            //Derive the output based on our template and view engine
            var html = viewContext.Writer.ToString();
            return new HtmlActionResult(html);
        }

        private HtmlActionResult RenderContentArea(IContent content, string name)
        {
            if (content == null)
            {
                throw new ContentNotFoundException("Content was not found");
            }

            var contentArea = content.Property[name].Value as ContentArea;
            if (contentArea == null)
            {
                throw new ContentNotFoundException("Content was not found");
            }

            //Resolve the right Template based on Episervers own templating engine
            var model = _templateResolver.Resolve(HttpContext.Current, content.GetOriginalType(), TemplateTypeCategories.Mvc | TemplateTypeCategories.MvcPartial, new string[0]);

            //Resolve the controller
            var contentController = ServiceLocator.Current.GetInstance(model.TemplateType) as ControllerBase;

            //Derive the name
            var controllerName = model.Name.Replace("Controller", "");

            //Mimic the routing of our rendition 
            var routeData = new RouteData();
            routeData.Values.Add("currentContent", content);
            routeData.Values.Add("controllerType", model.TemplateType);
            routeData.Values.Add("language", ContentLanguage.PreferredCulture.Name);
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("node", content.ContentLink.ID);

            var viewData = new ViewDataDictionary
            {
                { "HasContainer", false }
            };


            //Create a fake context, that can be executed based on the route
            var viewContext = new ViewContext(
                new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, contentController),
                new FakeView(),
                viewData,
                new TempDataDictionary(),
                new StringWriter());

            var helper = new HtmlHelper(viewContext, new ViewPage());

            //Render in our fake context
            _contentAreaRenderer.Render(helper, contentArea);

            //Derive the output based on our template and view engine
            var html = viewContext.Writer.ToString();
            return new HtmlActionResult(html);
        }

        public static ExpandoObject ConstructExpandoObject(IContent c, string @select = null)
        {
            return ConstructExpandoObject(c, true, @select);
        }

        public static ExpandoObject ConstructExpandoObject(IContent c, bool includeBinary, string @select = null)
        {
            dynamic e = new ExpandoObject();
            var dic = (IDictionary<string, object>) e;
            e.Name = c.Name;
            e.ParentLink = c.ParentLink;
            e.ContentGuid = c.ContentGuid;
            e.ContentLink = c.ContentLink;
            e.ContentTypeID = c.ContentTypeID;
            //TODO: Resolve Content Type
            var parts = select?.Split(',');

            var data = c as MediaData;
            if (data != null)
            {
                dynamic media = new ExpandoObject();
                var md = data;
                media.MimeType = md.MimeType;
                media.RouteSegment = md.RouteSegment;
                if (includeBinary)
                {
                    using (var br = new BinaryReader(md.BinaryData.OpenRead()))
                    {
                        media.Binary = Convert.ToBase64String(br.ReadBytes((int)br.BaseStream.Length));
                    }
                }
                dic.Add("Media", media);
            }

            foreach (var pi in c.Property)
            {
                if (parts != null && (!parts.Contains(pi.Name))) continue;

                if (pi.Value == null)
                {
                    continue;
                }
                if (pi.Type == PropertyDataType.Block)
                {
                    //TODO: Doesn't work. Check SiteLogoType on start page
                    if (pi.Value is IContent) dic.Add(pi.Name, ConstructExpandoObject((IContent)pi.Value));
                }
                else if (pi is EPiServer.SpecializedProperties.PropertyContentArea)
                {
                    //TODO: Loop through and make array
                    var pca = pi as EPiServer.SpecializedProperties.PropertyContentArea;
                    var ca = pca.Value as ContentArea;
                    if (ca != null)
                    {
                        dic.Add(pi.Name,
                            ca.Items.Select(itm => ConstructExpandoObject(itm.GetContent()))
                                .Select(itmobj => itmobj)
                                .ToArray());
                    }
                }
                else if (pi.Value is string[])
                {
                    dic.Add(pi.Name, ((string[]) pi.Value));
                }
                else if (pi.Value is Int32 || pi.Value is Boolean || pi.Value is DateTime || pi.Value is Double)
                {
                    dic.Add(pi.Name, pi.Value);
                }
                else
                {
                    //TODO: Handle different return values
                    dic.Add(pi.Name, (pi.Value != null) ? pi.ToWebString() : null);
                }
            }
            return e;
        }
    }

    public class FakeView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
        }
    }

    public class HtmlActionResult : IHttpActionResult
    {
        private readonly string _value;

        public HtmlActionResult(string value)
        {
            _value = value;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_value)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("text/html")
                    }
                }
            };

            return Task.FromResult(response);
        }
    }

}