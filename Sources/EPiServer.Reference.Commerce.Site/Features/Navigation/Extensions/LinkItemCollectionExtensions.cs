using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Routing;
using System;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Navigation.Extensions
{
    public static class LinkItemCollectionExtensions
    {
        private static readonly Lazy<IContentLoader> ContentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());
        private static readonly Lazy<UrlResolver> UrlResolver = new Lazy<UrlResolver>(() => ServiceLocator.Current.GetInstance<UrlResolver>());

        public static MegaMenuModel GetMegaMenuModel(this LinkItemCollection collection)
        {
            if (collection == null)
            {
                collection = new LinkItemCollection();
            }

            var model = new MegaMenuModel();
            //External Url
            var externalCollection = collection.ToList().Select((value, index) => new
            {
                value,
                index
            }).Where(x => !x.value.ReferencedPermanentLinkIds.Any());
            //Internal Url
            var contentReferences = collection.Where(x => x.ReferencedPermanentLinkIds.Any()).Select(x => x.GetContentReference()).ToList();

            //the OrderBy clause is required here because content will be ordered by the content provider that was used to retrieve the content,
            //meaning that all CMS content will be listed before all Commerce content, and so forth
            var items = ContentLoader.Value.GetItems(contentReferences, new LoaderOptions()).OrderBy(x => contentReferences.IndexOf(x.ContentLink));

            foreach (var item in items)
            {
                var firstLevelPage = item as SitePageData;
                var firstLevelNode = item as FashionNode;
                if (firstLevelPage != null && !firstLevelPage.VisibleInMenu)
                {
                    continue;
                }
                var firstLevel = new MegaMenuItem
                {
                    DisplayName = firstLevelPage != null ? item.Name : firstLevelNode?.DisplayName ?? item.Name,
                    Url = UrlResolver.Value.GetUrl(item.ContentLink),
                };
                model.MenuItems.Add(firstLevel);


                if (firstLevelPage != null)
                {
                    firstLevel.Description = firstLevelPage.TeaserText;
                    if (!ContentReference.IsNullOrEmpty(firstLevelPage.PageImage))
                    {
                        firstLevel.ImageUrl = UrlResolver.Value.GetUrl(firstLevelPage.PageImage);
                    }

                    var pageChildren = ContentLoader.Value.GetChildren<SitePageData>(firstLevelPage.ContentLink);
                    foreach (var pageChild in pageChildren)
                    {
                        if (!pageChild.VisibleInMenu)
                        {
                            continue;
                        }
                        var pageChildMenuItem = new MegaMenuItem
                        {
                            DisplayName = pageChild.Name,
                            Url = UrlResolver.Value.GetUrl(pageChild.ContentLink),
                            Description = pageChild.TeaserText
                        };
                        if (!ContentReference.IsNullOrEmpty(pageChild.PageImage))
                        {
                            pageChildMenuItem.ImageUrl = UrlResolver.Value.GetUrl(pageChild.PageImage);
                        }
                        firstLevel.Children.Add(pageChildMenuItem);


                        var pageChildrenOfNodeChild = ContentLoader.Value.GetChildren<SitePageData>(pageChild.ContentLink);
                        foreach (var pageChildOfChild in pageChildrenOfNodeChild)
                        {
                            if (!pageChildOfChild.VisibleInMenu)
                            {
                                continue;
                            }

                            var pageChildOfChildMenuItem = new MegaMenuItem
                            {
                                DisplayName = pageChildOfChild.Name,
                                Url = UrlResolver.Value.GetUrl(pageChildOfChild.ContentLink),
                                Description = pageChildOfChild.TeaserText
                            };
                            if (!ContentReference.IsNullOrEmpty(firstLevelPage.PageImage))
                            {
                                pageChildOfChildMenuItem.ImageUrl = UrlResolver.Value.GetUrl(firstLevelPage.PageImage);
                            }
                            pageChildMenuItem.Children.Add(pageChildOfChildMenuItem);
                        }
                    }
                }

                
                if (firstLevelNode == null)
                {
                    continue;
                }

                firstLevel.Description = firstLevelNode.Description?.ToHtmlString() ?? "";
                if (firstLevelNode.CommerceMediaCollection.Any())
                {
                    firstLevel.ImageUrl = UrlResolver.Value.GetUrl(firstLevelNode.CommerceMediaCollection.First().AssetLink);
                }
                var nodeChildren = ContentLoader.Value.GetChildren<FashionNode>(firstLevelNode.ContentLink);
                foreach (var nodeChild in nodeChildren)
                {
                    var nodeChildMenuItem = new MegaMenuItem
                    {
                        DisplayName = nodeChild.DisplayName,
                        Url = UrlResolver.Value.GetUrl(nodeChild.ContentLink),
                        Description = nodeChild.Description?.ToHtmlString() ?? ""
                    };
                    if (nodeChild.CommerceMediaCollection.Any())
                    {
                        nodeChildMenuItem.ImageUrl = UrlResolver.Value.GetUrl(nodeChild.CommerceMediaCollection.First().AssetLink);
                    }
                    firstLevel.Children.Add(nodeChildMenuItem);


                    var nodeChildrenOfNodeChild = ContentLoader.Value.GetChildren<FashionNode>(nodeChild.ContentLink);
                    foreach (var nodeChildOfChild in nodeChildrenOfNodeChild)
                    {
                        var nodeChildOfChildMenuItem = new MegaMenuItem
                        {
                            DisplayName = nodeChildOfChild.DisplayName,
                            Url = UrlResolver.Value.GetUrl(nodeChildOfChild.ContentLink),
                            Description = nodeChildOfChild.Description?.ToHtmlString() ?? ""
                        };
                        if (nodeChildOfChild.CommerceMediaCollection.Any())
                        {
                            nodeChildOfChildMenuItem.ImageUrl = UrlResolver.Value.GetUrl(nodeChildOfChild.CommerceMediaCollection.First().AssetLink);
                        }
                        nodeChildMenuItem.Children.Add(nodeChildOfChildMenuItem);
                    }
                }

            }
            foreach (var external in externalCollection)
            {
                var index = external.index > model.MenuItems.Count() ? model.MenuItems.Count() : external.index;
                model.MenuItems.Insert(index, new MegaMenuItem
                {
                    DisplayName = external.value.Text,
                    Url = external.value.Href
                });
            }
            return model;
        }
    }
}