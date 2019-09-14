using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.B2B.Models.Pages;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Pages;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderDetails.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderPads.Pages;
using EPiServer.Reference.Commerce.Site.Features.QuickOrder.Pages;
using EPiServer.Reference.Commerce.Site.Features.ResetPassword.Pages;
using EPiServer.Reference.Commerce.Site.Features.Search.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.SubOrganization.Pages;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Reference.Commerce.Site.Features.Start.Helper;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Models
{
    public abstract partial class BaseStartPage : SitePageData
    {
        #region Site Settings

        [Display(Name = "Header Logo", GroupName = SiteTabs.SiteSettings, Order = 1)]
        [UIHint(UIHint.Image)]
        [CultureSpecific]
        public virtual ContentReference HeaderLogo { get; set; }

        [Display(Name = "Header Banner Text", GroupName = SiteTabs.SiteSettings, Order = 2)]
        public virtual XhtmlString HeaderBannerText { get; set; }

        [Display(Name = "Show Commerce Header Components", GroupName = SiteTabs.SiteSettings, Order = 3)]
        public virtual bool ShowCommerceHeaderComponents { get; set; }

        [Display(Name = "Show Product Ratings on all product tiles", GroupName = SiteTabs.SiteSettings, Order = 4)]
        public virtual bool ShowProductRatingsOnListings { get; set; }

        [Display(Name = "Show Share this", GroupName = SiteTabs.SiteSettings, Order = 100)]
        public virtual bool ShowShareThis { get; set; }

        [SelectOne(SelectionFactoryType = typeof(HeaderMenuSelectionFactory))]
        [Display(Name = "Header menu style", GroupName = SiteTabs.SiteSettings, Order = 5)]
        public virtual string HeaderMenuStyle { get; set; }

        [Display(Name = "Quick Logins From Top Right Dropdown", GroupName = SiteTabs.SiteSettings, Order = 300)]
        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<SwitchableUser>))]
        public virtual System.Collections.Generic.IList<SwitchableUser> QuickAccessLogins { get; set; }
        #endregion

        #region Content

        [CultureSpecific]
        [Display(
            Name = "Start title",
            Description = "Title for the start page",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Title format",
            Description = "To suffix the title on each page, use {title} - yoursuffix. Also supports prefix in the same manner. Affects entire site except from the start page itself.",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual string TitleFormat { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Top content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual ContentArea TopContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Main content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 80)]
        public virtual ContentArea MainContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Bottom content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 5)]
        public virtual ContentArea BottomContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Notifications",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 6)]
        [AllowedTypes(new[] { typeof(NotificationBlock) })]
        public virtual ContentArea Notifications { get; set; }

        #endregion

        #region Search Settings

        [SelectOne(SelectionFactoryType = typeof(SearchOptionSelectionFactory))]
        [Display(Name = "Search option", GroupName = SiteTabs.SearchSettings, Order = 50)]
        public virtual string SearchOption { get; set; }

        [Display(Name = "Show products in search results", GroupName = SiteTabs.SearchSettings, Order = 100)]
        public virtual bool ShowProductSearchResults { get; set; }

        [Display(Name = "Show articles in search results", GroupName = SiteTabs.SearchSettings, Order = 150)]
        public virtual bool ShowArticleSearchResults { get; set; }

        [Display(Name = "Show PDF files in search results", GroupName = SiteTabs.SearchSettings, Order = 200)]
        public virtual bool ShowPDFSearchResults { get; set; }

        [SelectOne(SelectionFactoryType = typeof(FindCatalogSelectionFactory))]
        [Display(Name = "Search Catalog", GroupName = SiteTabs.SearchSettings, Order = 250, Description = "The catalogs that will be returned by search.")]
        public virtual int SearchCatalog { get; set; }

        #endregion

        #region Menu

        [Display(Name = "Do not show SVG dropdown in the top-nav", GroupName = SiteTabs.Menu, Order = 1)]
        public virtual bool DoNotShowSvgDropdown { get; set; }

        [Display(Name = "Header Meta Navigation", GroupName = SiteTabs.Menu, Order = 11)]
        [UIHint("HeaderMetaNavigation")]
        public virtual LinkItemCollection HeaderMetaNavigation { get; set; }

        [Display(Name = "Header Main Navigation", GroupName = SiteTabs.Menu, Order = 12)]
        [UIHint("HeaderMainNavigation")]
        public virtual LinkItemCollection HeaderMainNavigation { get; set; }

        [CultureSpecific]
        [Display(
            Name = "My Account menu",
            Description = "",
            GroupName = SiteTabs.Menu,
            Order = 5)]
        public virtual LinkItemCollection MyAccountMenu { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Basic Account menu",
            Description = "",
            GroupName = SiteTabs.Menu,
            Order = 5)]
        public virtual LinkItemCollection BasicAccountMenu { get; set; }

        [CultureSpecific]
        [Display(
            Name = "B2B menu",
            Description = "",
            GroupName = SiteTabs.Menu,
            Order = 6)]
        public virtual LinkItemCollection B2BMenu { get; set; }

        [Display(Name = "B2B Header Main Navigation", GroupName = SiteTabs.Menu, Order = 12)]
        public virtual LinkItemCollection B2BHeaderMainNavigation { get; set; }

        [Display(Name = "Mobile Navigation Pages", GroupName = SiteTabs.Menu, Order = 13)]
        public virtual LinkItemCollection MobileNavigationPages { get; set; }

        #endregion

        #region Footer

        [CultureSpecific]
        [Display(
            Name = "Bottom feature box",
            GroupName = SiteTabs.Footer,
            Order = 10)]
        public virtual ContentArea BottomFeatureBox { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Bottom email subscribe",
            GroupName = SiteTabs.Footer,
            Order = 20)]
        public virtual ContentArea BottomEmailSubscribe { get; set; }

        // TODO: The footer stuff needs to be changed to be more user-friendly and flexible
        [Display(Name = "Footer Column 1 Header", GroupName = SiteTabs.Footer, Order = 30)]
        public virtual string FooterColumnHeader1 { get; set; }

        [Display(Name = "Footer Column 1 Navigation", GroupName = SiteTabs.Footer, Order = 40)]
        //[UIHint("FooterColumnNavigation")]
        public virtual ContentArea FooterColumnNavigation1 { get; set; }

        [Display(Name = "Footer Column 2 Header", GroupName = SiteTabs.Footer, Order = 50)]
        public virtual string FooterColumnHeader2 { get; set; }

        [Display(Name = "Footer Column 2 Navigation", GroupName = SiteTabs.Footer, Order = 60)]
        [UIHint("FooterColumnNavigation")]
        public virtual LinkItemCollection FooterColumnNavigation2 { get; set; }

        [Display(Name = "Footer Column 3 Header", GroupName = SiteTabs.Footer, Order = 70)]
        public virtual string FooterColumnHeader3 { get; set; }

        [Display(Name = "Footer Column 3 Navigation", GroupName = SiteTabs.Footer, Order = 80)]
        [UIHint("FooterColumnNavigation")]
        public virtual LinkItemCollection FooterColumnNavigation3 { get; set; }

        [Display(Name = "Footer Logo", GroupName = SiteTabs.Footer, Order = 90)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference FooterLogo { get; set; }

        [Display(Name = "Footer Comapny Address", GroupName = SiteTabs.Footer, Order = 100)]
        public virtual string FooterCompanyAddress { get; set; }

        [Display(Name = "Footer Comapny Phone", GroupName = SiteTabs.Footer, Order = 110)]
        public virtual string FooterCompanyPhone { get; set; }

        [Display(Name = "Footer Company Email", GroupName = SiteTabs.Footer, Order = 120)]
        public virtual string FooterCompanyEmail { get; set; }

        [Display(Name = "Footer Copyright", GroupName = SiteTabs.Footer, Order = 130)]
        public virtual string FooterCopyrightText { get; set; }

        [Display(Name = "Footer Copyright Navigation", GroupName = SiteTabs.Footer, Order = 140)]
        [UIHint("FooterCopyrightNavigation")]
        public virtual LinkItemCollection FooterCopyrightNavigation { get; set; }

        #endregion

        #region Site Structure

        [Display(
            Name = "Checkout page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 1)]
        [AllowedTypes(typeof(CheckoutPage))]
        public virtual ContentReference CheckoutPage { get; set; }

        [Display(
            Name = "Store locator",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 2)]
        public virtual ContentReference StoreLocator { get; set; }

        [Display(
            Name = "Address book page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 3)]
        [AllowedTypes(typeof(AddressBookPage))]
        public virtual ContentReference AddressBookPage { get; set; }

        [Display(
            Name = "Wish list page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 4)]
        [AllowedTypes(typeof(WishListPage))]
        public virtual ContentReference WishListPage { get; set; }

        [Display(
            Name = "Shared Cart Page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 4)]
        [AllowedTypes(typeof(SharedCartPage))]
        public virtual ContentReference SharedCartPage { get; set; }

        [Display(
            Name = "Search page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 5)]
        [AllowedTypes(typeof(SearchPage))]
        public virtual ContentReference SearchPage { get; set; }

        [Display(
            Name = "Reset password page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 6)]
        [AllowedTypes(typeof(ResetPasswordPage))]
        public virtual ContentReference ResetPasswordPage { get; set; }

        [Display(
            Name = "Payment plan details page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 7)]
        [AllowedTypes(typeof(PaymentPlanDetailPage))]
        public virtual ContentReference PaymentPlanDetailsPage { get; set; }

        [Display(
            Name = "Cart page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 8)]
        [AllowedTypes(typeof(CartPage))]
        public virtual ContentReference CartPage { get; set; }

        [Display(
            Name = "Payment plan history page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 9)]
        [AllowedTypes(typeof(PaymentPlanHistoryPage))]
        public virtual ContentReference PaymentPlanHistoryPage { get; set; }

        [Display(
            Name = "Organization Main Page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 10)]
        [AllowedTypes(typeof(OrganizationPage))]
        public virtual ContentReference OrganizationMainPage { get; set; }

        [Display(
            Name = "Sub Organization Page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 11)]
        [AllowedTypes(typeof(SubOrganizationPage))]
        public virtual ContentReference SubOrganizationPage { get; set; }

        [Display(
            Name = "Organization Pads Page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 12)]
        [AllowedTypes(typeof(OrderPadsPage))]
        public virtual ContentReference OrderPadsPage { get; set; }

        [Display(
            Name = "Quick Orders Page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 13)]
        [AllowedTypes(typeof(QuickOrderPage))]
        public virtual ContentReference QuickOrderPage { get; set; }

        [Display(
                Name = "Resource not found page",
                Description = "",
                GroupName = SiteTabs.SiteStructure,
                Order = 14)]
        public virtual ContentReference PageNotFound { get; set; }

        [Display(
            Name = "Order details page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 15)]
        [AllowedTypes(typeof(OrderDetailsPage))]
        public virtual ContentReference OrderDetailsPage { get; set; }

        [Display(
            Name = "Order history page",
            Description = "",
            GroupName = SiteTabs.SiteStructure,
            Order = 16)]
        [AllowedTypes(typeof(OrderHistoryPage))]
        public virtual ContentReference OrderHistoryPage { get; set; }

        #endregion

        #region Mail templates

        [Display(
         Name = "Order confirmation mail",
         Description = "",
         GroupName = SiteTabs.MailTemplates,
         Order = 1)]
        [AllowedTypes(typeof(OrderConfirmationMailPage))]
        public virtual ContentReference OrderConfirmationMail { get; set; }

        [Display(
         Name = "Send Order Confirmation Mail",
         Description = "",
         GroupName = SiteTabs.MailTemplates,
         Order = 2)]
        public virtual bool SendOrderConfirmationMail { get; set; }

        [Display(
            Name = "Reset password mail",
            Description = "",
            GroupName = SiteTabs.MailTemplates,
            Order = 3)]
        public virtual ContentReference ResetPasswordMail { get; set; }

        #endregion

        #region Labels
        [Display(Name = "My Account Label",
            GroupName = SiteTabs.Labels,
            Order = 1)]
        public virtual string MyAccountLabel { get; set; }

        [Display(Name = "Cart Label",
            GroupName = SiteTabs.Labels,
            Order = 2)]
        public virtual string CartLabel { get; set; }

        [Display(Name = "Search Label",
            GroupName = SiteTabs.Labels,
            Order = 3)]
        public virtual string SearchLabel { get; set; }

        [Display(Name = "Wishlist Label",
            GroupName = SiteTabs.Labels,
            Order = 4)]
        public virtual string WishlistLabel { get; set; }

        [Display(Name = "Shared Cart Label",
            GroupName = SiteTabs.Labels,
            Order = 5)]
        public virtual string SharedCartLabel { get; set; }
        #endregion

        #region Insight Visitor Group Tracking

        [Display(Name = "Visitor Groups To Track",
            Description = "All selected groups will be tracked in Insight if they are matched on any page on the site",
            GroupName = "Insight tracking",
            Order = 10000)]
        [SelectMany(SelectionFactoryType = typeof(VisitorGroupSelectionFactory))]
        public virtual string VisitorGroupsToTrack { get; set; }

        #endregion

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            SearchCatalog = 0;
            ShowShareThis = false;
            SendOrderConfirmationMail = false;
        }
    }
}