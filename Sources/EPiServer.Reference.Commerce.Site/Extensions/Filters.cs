using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Querying.Filters;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class Filters
    {
        public static DelegateFilterBuilder Prefix(this IEnumerable<string> value, string prefix)
        {
            return new DelegateFilterBuilder(field => new PrefixFilter(field, prefix));
        }

        public static DelegateFilterBuilder PrefixCaseInsensitive(this IEnumerable<string> value, string prefix)
        {
            return new DelegateFilterBuilder(field => new PrefixFilter(field, prefix.ToLowerInvariant()))
            {
                FieldNameMethod =
                    (expression, conventions) =>
                    conventions.FieldNameConvention.GetFieldNameForLowercase(expression)
            };
        }

        public static FilterBuilder<T> FilterOutline<T>(this FilterBuilder<T> filterBuilder,
            IEnumerable<string> value)
        {
            var outlineFilterBuilder = new FilterBuilder<ContentData>(filterBuilder.Client);
            outlineFilterBuilder = outlineFilterBuilder.And(x => !x.MatchTypeHierarchy(typeof(EntryContentBase)));
            outlineFilterBuilder = value.Aggregate(outlineFilterBuilder, (current, filter) => current.Or(x => ((EntryContentBase)x).Outline().PrefixCaseInsensitive(filter)));
            return filterBuilder.And(x => outlineFilterBuilder);
        }

        public static ITypeSearch<T> FilterOutline<T>(this ITypeSearch<T> search, IEnumerable<string> value)
        {
            var filterBuilder = new FilterBuilder<T>(search.Client)
                .FilterOutline(value);

            return search.Filter(x => filterBuilder);
        }
    }
}