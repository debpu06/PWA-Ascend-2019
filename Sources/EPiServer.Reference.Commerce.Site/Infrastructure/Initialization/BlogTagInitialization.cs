using System;
using System.Linq;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Blog;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Globalization;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class BlogTagInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();

            events.CreatingContent += CreatingContent;

            var partialRouter = new BlogPartialRouter();

            RouteTable.Routes.RegisterPartialRouter<BlogStartPage, Category>(partialRouter);

        }

        private void CreatingContent(object sender, ContentEventArgs e)
        {
            if (IsImport() || !(e.Content is PageData))
            {
                return;
            }

            var page = e.Content as BlogItemPage;
            if (page == null)
            {
                return;
            }

            var startPublish = DateTime.UtcNow;
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var parentPage = contentRepository.Get<PageData>(page.ParentLink);
            if (parentPage is BlogStartPage)
            {
                page.ParentLink = GetDatePageRef(parentPage, startPublish, contentRepository);
            }

            page.TagCloud.Heading = "Tags";

            var blog = contentRepository.GetChildren<BlogStartPage>(ContentReference.StartPage).
                FirstOrDefault();

            if (blog == null)
            {
                return;
            }
            page.Archive.BlogStart = blog.ContentLink;
            page.TagCloud.BlogTagLinkPage = blog.ContentLink;
        }

        public bool IsImport()
        {
            return false;
        }

        public PageReference GetDatePageRef(PageData blogStart, DateTime published, IContentRepository contentRepository)
        {

            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(published.Month);
            foreach (var current in contentRepository.GetChildren<PageData>(blogStart.ContentLink))
            {
                if (current.Name != published.Year.ToString())
                {
                    continue;
                }
                PageReference result;
                foreach (var current2 in contentRepository.GetChildren<PageData>(current.ContentLink))
                {
                    if (current2.Name != monthName)
                    {
                        continue;
                    }
                    result = current2.PageLink;
                    return result;
                }
                result = CreateDatePage(contentRepository, current.PageLink, monthName, new DateTime(published.Year, published.Month, 1));
                return result;
            }
            var parent = CreateDatePage(contentRepository, blogStart.ContentLink, published.Year.ToString(), new DateTime(published.Year, 1, 1));
            return CreateDatePage(contentRepository, parent, monthName, new DateTime(published.Year, published.Month, 1));
        }

        private PageReference CreateDatePage(IContentRepository contentRepository, ContentReference parent, string name, DateTime startPublish)
        {
            var defaultPageData = contentRepository.GetDefault<BlogListPage>(parent, typeof(BlogListPage).GetPageType().ID);
            defaultPageData.PageName = name;
            defaultPageData.Heading = name;
            defaultPageData.StartPublish = startPublish;
            var urlSegment = ServiceLocator.Current.GetInstance<IUrlSegmentCreator>();
            defaultPageData.URLSegment = urlSegment.Create(defaultPageData);
            
            contentRepository.Save(defaultPageData, SaveAction.Publish, AccessLevel.Publish);
            defaultPageData = (defaultPageData.CreateWritableClone() as BlogListPage);
            defaultPageData.Archive.BlogStart = defaultPageData.ContentLink;
            defaultPageData.TagCloud.BlogTagLinkPage = defaultPageData.ContentLink;
            defaultPageData.TagCloud.Heading = "Tags";
            contentRepository.Save(defaultPageData, SaveAction.Publish, AccessLevel.Publish);
            return defaultPageData.PageLink;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            var events = ServiceLocator.Current.GetInstance<IContentEvents>();

            events.CreatingContent -= CreatingContent;

        }
    }
}
