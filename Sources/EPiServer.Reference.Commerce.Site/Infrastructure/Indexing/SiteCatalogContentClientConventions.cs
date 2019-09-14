using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Find.Commerce;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Indexing
{
    public class SiteCatalogContentClientConventions : CatalogContentClientConventions
    {
        protected override void ApplyProductContentConventions(Find.ClientConventions.TypeConventionBuilder<ProductContent> conventionBuilder)
        {
            base.ApplyProductContentConventions(conventionBuilder);

            conventionBuilder.IncludeField(x => x.DefaultPrice())
                .IncludeField(x => x.Prices())
                .IncludeField(x => x.Inventories())
                .IncludeField(x => x.Outline())
                .IncludeField(x => x.SortOrder());
        }

        protected override void ApplyBundleContentConventions(Find.ClientConventions.TypeConventionBuilder<BundleContent> conventionBuilder)
        {
            base.ApplyBundleContentConventions(conventionBuilder);

            conventionBuilder.IncludeField(x => x.DefaultPrice())
                .IncludeField(x => x.Prices())
                .IncludeField(x => x.Inventories())
                .IncludeField(x => x.Outline())
                .IncludeField(x => x.SortOrder());
        }

        protected override void ApplyPackageContentConventions(Find.ClientConventions.TypeConventionBuilder<PackageContent> conventionBuilder)
        {
            base.ApplyPackageContentConventions(conventionBuilder);
            conventionBuilder.ExcludeField(x => IPricingExtensions.DefaultPrice(x));
            conventionBuilder.IncludeField(x => Extensions.ProductContentExtensions.DefaultPrice(x))
                .IncludeField(x => x.Outline())
                .IncludeField(x => x.SortOrder());
        }

        public override void ApplyConventions(IClientConventions clientConventions)
        {
            base.ApplyConventions(clientConventions);
            ContentIndexer.Instance.Conventions.ForInstancesOf<FashionVariant>().ShouldIndex(x => false);
        }

    }
}