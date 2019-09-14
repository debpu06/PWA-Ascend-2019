using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class CategoryPartialController : PartialContentController<BaseNode>
    {
        private readonly SearchViewModelFactory _viewModelFactory;

        public CategoryPartialController(SearchViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public override ActionResult Index(BaseNode currentContent)
        {
            var viewmodel = GetSearchModel(currentContent);
            return PartialView("_Category", viewmodel);
        }

        protected virtual SearchViewModel<BaseNode> GetSearchModel(BaseNode currentContent)
        {
            return _viewModelFactory.Create(currentContent, new FilterOptionViewModel
            {
                FacetGroups = new List<FacetGroupOption>(),
                Page = 1,
                PageSize = currentContent.PartialPageSize
            }, HttpContext.Request.QueryString["facets"]);
        }
    }
}