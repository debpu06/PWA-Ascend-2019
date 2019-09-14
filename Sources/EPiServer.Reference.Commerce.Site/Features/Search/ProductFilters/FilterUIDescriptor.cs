using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Features.Search.ProductFilters
{
    [UIDescriptorRegistration]
    public class CatalogContentUiDescriptor : UIDescriptor<FilterBaseBlock>
    {
        public CatalogContentUiDescriptor()
        {
            DefaultView = CmsViewNames.AllPropertiesView;

        }
    }
}