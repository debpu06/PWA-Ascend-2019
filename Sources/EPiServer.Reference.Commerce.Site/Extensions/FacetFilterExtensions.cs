using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Helpers;
using EPiServer.Reference.Commerce.Site.Features.Facets;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class FacetFilterExtensions
    {
        public static ITypeSearch<TSource> TermsFacetFor<TSource>(this ITypeSearch<TSource> search,
            string name,
            Type type,
            Filter filter,
            Action<FacetFilterRequest> facetRequestAction = null,
            int size = 50)
        {
            var fieldName = name;
            if (type != null)
            {
                fieldName = search.Client.GetFullFieldName(name, type);
            }
            return new Search<TSource, IQuery>(search,
                context =>
                {
                    var facetRequest = new TermsFacetFilterRequest(name, filter)
                    {
                        Field = fieldName,
                        Size = size
                    };
                    if (facetRequestAction.IsNotNull())
                    {
                        facetRequestAction(facetRequest);
                    }
                    context.RequestBody.Facets.Add(facetRequest);
                });
        }

        public static ITypeSearch<TSource> RangeFacetFor<TSource>(this ITypeSearch<TSource> search,
            string name,
            Type type,
            Filter filter,
            IEnumerable<NumericRange> range,
            Action<RangeFacetFilterRequest> facetRequestAction = null)
        {
            var fieldName = search.Client.GetFullFieldName(name, type);
            ;
            var action = NumericRangfeFacetRequestAction(search.Client, name, range, type);
            return new Search<TSource, IQuery>(search,
                context =>
                {
                    var facetRequest = new RangeFacetFilterRequest(name, filter)
                    {
                        Field = fieldName
                    };
                    action(facetRequest);
                    context.RequestBody.Facets.Add(facetRequest);
                });
        }

        private static Action<RangeFacetFilterRequest> NumericRangfeFacetRequestAction(IClient searchClient,
            string fieldName,
            IEnumerable<NumericRange> range,
            Type type)
        {
            var name = searchClient.GetFullFieldName(fieldName, type);

            return x =>
            {
                x.Field = name;
                x.Ranges.AddRange(range);
            };
        }
    }
}