using System;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.VisitorGroupUIStyling
{
    public class OverrideVisitorGroupCssAttribute : OutputProcessorActionFilterAttribute
    {
        protected override string Process(string data)
        {
            return data
                .Replace("</head", "<link href=\"/ClientResources/Styles/VisitorGroupUIOverrides.css\" rel=\"stylesheet\"></link></head");
        }

        protected override bool ShouldProcess(ResultExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResultBase;
            var view = result?.View as WebFormView;
            return view != null 
                && (view.ViewPath.Equals("/EPiServer/CMS/Views/VisitorGroups/Index.aspx", StringComparison.InvariantCultureIgnoreCase) || view.ViewPath.Equals("/EPiServer/CMS/Views/VisitorGroups/Edit.aspx", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}