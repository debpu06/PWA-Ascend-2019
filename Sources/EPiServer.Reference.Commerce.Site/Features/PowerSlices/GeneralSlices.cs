﻿using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Find;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Shell.Rest;
using EPiServer.Shell.Services.Rest;
using PowerSlice;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.PowerSlices
{
    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class EverythingSlice : ContentSliceBase<IContent>
    {
        public override string Name
        {
            get { return "Everything"; }
        }

        public override int SortOrder
        {
            get
            {
                return 1;
            }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class MyContentSlice : ContentSliceBase<IContent>
    {
        public override string Name
        {
            get { return "My content"; }
        }

        protected override ITypeSearch<IContent> Filter(ITypeSearch<IContent> searchRequest, ContentQueryParameters parameters)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            return searchRequest.Filter(x => x.MatchTypeHierarchy(typeof(IChangeTrackable)) & ((IChangeTrackable)x).CreatedBy.Match(userName));
        }

        public override int SortOrder
        {
            get
            {
                return 2;
            }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class MyPagesSlice : ContentSliceBase<SitePageData>
    {
        public override string Name
        {
            get { return "My pages"; }
        }

        protected override ITypeSearch<SitePageData> Filter(ITypeSearch<SitePageData> searchRequest, ContentQueryParameters parameters)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            return searchRequest.Filter(x => x.MatchTypeHierarchy(typeof(IChangeTrackable)) & ((IChangeTrackable)x).CreatedBy.Match(userName));
        }

        public override int SortOrder
        {
            get
            {
                return 3;
            }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class UnusedMediaSlice : ContentSliceBase<MediaData>
    {
        protected IContentRepository ContentRepository;

        public UnusedMediaSlice(IClient searchClient, IContentTypeRepository contentTypeRepository, IContentLoader contentLoader, IContentRepository contentRepository)
            : base(searchClient, contentTypeRepository, contentLoader)
        {
            ContentRepository = contentRepository;
        }

        public override string Name
        {
            get { return "Unused Media"; }
        }

        public override QueryRange<IContent> ExecuteQuery(IQueryParameters parameters)
        {
            var originalContentRange = base.ExecuteQuery(parameters);
            var filteredResults = originalContentRange.Items.Where(IsNotReferenced).ToList();

            var itemRange = new ItemRange
            {
                Total = filteredResults.Count,
                Start = parameters.Range.Start,
                End = parameters.Range.End
            };

            return new ContentRange(filteredResults, itemRange);
        }

        protected bool IsNotReferenced(IContent content)
        {
            return !ContentRepository.GetReferencesToContent(content.ContentLink, false).Any();
        }

        public override int SortOrder
        {
            get
            {
                return 200;
            }
        }
    }

    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class UnusedBlocksSlice : ContentSliceBase<BlockData>
    {
        protected IContentRepository ContentRepository;

        public UnusedBlocksSlice(IClient searchClient, IContentTypeRepository contentTypeRepository, IContentLoader contentLoader, IContentRepository contentRepository) : base(searchClient, contentTypeRepository, contentLoader)
        {
            ContentRepository = contentRepository;
        }

        public override string Name
        {
            get { return "Unused Blocks"; }
        }

        public override QueryRange<IContent> ExecuteQuery(IQueryParameters parameters)
        {
            var originalContentRange = base.ExecuteQuery(parameters);
            var filteredResults = originalContentRange.Items.Where(IsNotReferenced).ToList();

            var itemRange = new ItemRange
            {
                Total = filteredResults.Count,
                Start = parameters.Range.Start,
                End = parameters.Range.End
            };

            return new ContentRange(filteredResults, itemRange);
        }

        protected bool IsNotReferenced(IContent content)
        {
            return !ContentRepository.GetReferencesToContent(content.ContentLink, false).Any();
        }

        public override int SortOrder
        {
            get
            {
                return 201;
            }
        }

    }
}