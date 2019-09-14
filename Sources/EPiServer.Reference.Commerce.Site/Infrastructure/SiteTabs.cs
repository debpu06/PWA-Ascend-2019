using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Security;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    [GroupDefinitions]
    public static class SiteTabs
    {
        [Display(Name = "Contact", Order = 1)]
        public const string Contact = "Contact";

        [Display(Name = "Default", Order = 2)]
        public const string Default = "Default";

        [Display(Name = "Metadata", Order = 3)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string MetaData = "Metadata";

        [Display(Name = "News", Order = 4)]
        public const string News = "News";

        [Display(Name = "Products", Order = 5)]
        public const string Products = "Products";

        [Display(Name = "Site Settings", Order = 6)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string SiteSettings = "SiteSettings";

        [Display(Name = "Specialized", Order = 998)]
        public const string Specialized = "Specialized";

        [Display(Name = "Search Settings", Order = 7)]
        public const string SearchSettings = "Search Settings";

        [Display(Name = "Location", Order = 8)]
        public const string Location = "Location";

        [Display(Order = 0)]
        public const string Content = SystemTabNames.Content;

        [Display(Name = "Teaser", Order = 10)]
        public const string Teaser = "Teaser";

        [Display(Name = "Review", Order = 11)]
        public const string Review = "Review";

        [Display(Name = "Styles", Order = 12)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string Styles = "Styles";

        [Display(Name = "Scripts", Order = 13)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string Scripts = "Scripts";

        [Display(Name = "Menu", Order = 14)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string Menu = "Menu";

        [Display(Name = "Footer", Order = 15)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string Footer = "Footer";

        [Display(Name = "Wallpaper", Order = 999)]
        public const string Wallpaper = "Wallpaper";

        [Display(Order = 100)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string SiteStructure = "Site structure";

        [Display(Order = 110)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string MailTemplates = "Mail templates";

        [Display(Order = 16)]
        [RequiredAccess(AccessLevel.Edit)]
        public const string Labels = "Labels";


        [Display(Order = 996)]
        public const string Social = "Social";
    }
}