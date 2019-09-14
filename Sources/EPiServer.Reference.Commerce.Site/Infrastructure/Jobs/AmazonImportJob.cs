using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Jobs.Models;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.Web;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.InventoryService;
using Mediachase.Commerce.Pricing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using EPiServer.Framework.Blobs;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Inventory;
using EPiServer.Commerce.Catalog.Linking;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Jobs
{
    [ScheduledPlugIn(DisplayName = "Amazon Import Job", GUID = "6AEE0703-2163-49AE-AE49-5723212B8A3D", Description = "Imports catalog output file from amazon catalog builder")]
    public class AmazonImportJob : ScheduledJobBase
    {
        private bool _stopSignaled;
        private ContentReference _contentImageFolder;
        private readonly IContentRepository _contentRepository;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IInventoryService _inventoryService;
        private readonly IPriceDetailService _priceDetailService;
        private readonly IUrlSegmentGenerator _urlSegmentGenerator;
        private readonly ContentReference _mediaRootFolder;
        private readonly IBlobFactory _blobFactory;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly Random _random = new Random();
        private readonly UniqueValueGenerator _uniqueValueGenerator;
        private readonly IRelationRepository _relationRepository;

        public AmazonImportJob(IContentRepository contentRepository,
            ReferenceConverter referenceConverter,
            IInventoryService inventoryService,
            IPriceDetailService priceDetailService,
            IUrlSegmentGenerator urlSegmentGenerator,
            IBlobFactory blobFactory,
            IWarehouseRepository warehouseRepository,
            UniqueValueGenerator uniqueValueGenerator,
            IRelationRepository relationRepository)
        {
            _contentRepository = contentRepository;
            _referenceConverter = referenceConverter;
            _inventoryService = inventoryService;
            _priceDetailService = priceDetailService;
            _mediaRootFolder = SiteDefinition.Current.GlobalAssetsRoot;
            _urlSegmentGenerator = urlSegmentGenerator;
            _blobFactory = blobFactory;
            _warehouseRepository = warehouseRepository;
            _uniqueValueGenerator = uniqueValueGenerator;
            _relationRepository = relationRepository;
            IsStoppable = true;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            //Call OnStatusChanged to periodically notify progress of job for manually started jobs
            OnStatusChanged($"Starting execution of {this.GetType()}");

            var catalogFile = ConfigurationManager.AppSettings["AmazonImportCatalogFile"];
            if (string.IsNullOrEmpty(catalogFile))
            {
                throw new EPiServerException("Appsetting AmazonImportCatalogFile is missing");
            }

            var fileInfo = new FileInfo(catalogFile);
            if (!fileInfo.Exists)
            {
                throw new EPiServerException($"Catalog file {catalogFile} does not exist.");
            }
            var catalogNameParts = fileInfo.Name.Split('_');
            if (catalogNameParts.Length != 2)
            {
                throw new EPiServerException($"Catalog file {catalogFile} is in wrong format, please do not change output file from amazon catalog builder tool.");
            }
            //Add implementation
            CleanupCatalog(catalogNameParts.First());
            _contentImageFolder = GetOrCreateContentFolder($"Catalog/{catalogNameParts.First()}");
            var count = RunCatalogImport(catalogFile, catalogNameParts.First());

            //For long running jobs periodically check if stop is signaled and if so stop execution
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }

            return $"Job completed importing {count} entries";
        }

        private void CleanupCatalog(string catalogName)
        {
            var catalog = _contentRepository.GetChildren<CatalogContent>(_referenceConverter.GetRootLink()).
                FirstOrDefault(x => x.Name.Equals(catalogName));

            if (catalog == null)
            {
                return;
            }

            _contentRepository.Delete(catalog.ContentLink, true, AccessLevel.NoAccess);

        }

        private int RunCatalogImport(string catalogFilename, string catalogName)
        {

            var catalog = _contentRepository.GetChildren<CatalogContent>(_referenceConverter.GetRootLink()).
                FirstOrDefault(x => x.Name.Equals(catalogName));
            if (catalog == null)
            {
                catalog = _contentRepository.GetDefault<CatalogContent>(_referenceConverter.GetRootLink());
                catalog.CatalogLanguages = new ItemCollection<string>(new[] { "en", "sv" });
                catalog.DefaultCurrency = "USD";
                catalog.DefaultLanguage = "en";
                catalog.IsPrimary = true;
                catalog.LengthBase = "in";
                catalog.WeightBase = "lbs";
                catalog.Name = catalogName;
                catalog.StartPublish = DateTime.UtcNow;
                catalog.StopPublish = DateTime.UtcNow.AddYears(10);
                _contentRepository.Save(catalog, SaveAction.Publish, AccessLevel.NoAccess);
            }

            List<AmazonProduct> products;
            using (var sr = new StreamReader(catalogFilename))
            {
                var json = sr.ReadToEnd();
                products = JsonConvert.DeserializeObject<List<AmazonProduct>>(json,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            var warehouses = _warehouseRepository.List().Where(x => x.IsActive).ToList();
            var productCount = 0;
            var totalCount = products.Count + products.Sum(x => x.Variations?.Variations?.Count);

            foreach (var amazonProduct in products)
            {
                if (productCount % 25 == 0)
                {
                    OnStatusChanged($"Importing products {productCount} of {totalCount}");
                }
                if (_stopSignaled)
                {
                    break;
                }

                if (amazonProduct.Variations?.Variations == null || !amazonProduct.Variations.Variations.Any())
                {
                    continue;
                }

                var productNodes = new List<FashionNode>();
                var productVariants = new List<ProductVariation>();
                foreach (var nodeName in amazonProduct.NodePath)
                {
                    var parentLink = productNodes.Any() ? productNodes.Last().ContentLink : catalog.ContentLink;
                    var code = _urlSegmentGenerator.Create(nodeName, new UrlSegmentOptions { UseLowercase = false });
                    var catalogNode = _contentRepository.GetChildren<FashionNode>(parentLink).
                        FirstOrDefault(x => x.Name.Equals(code));

                    if (catalogNode != null)
                    {
                        productNodes.Add(catalogNode);
                        continue;
                    }
                    catalogNode = _contentRepository.GetDefault<FashionNode>(parentLink);
                    catalogNode.Name = code;
                    catalogNode.Code = _uniqueValueGenerator.GenerateCode(code);
                    catalogNode.DisplayName = code;
                    _contentRepository.Save(catalogNode, SaveAction.Publish, AccessLevel.NoAccess);
                    productNodes.Add(catalogNode);
                }

                var productNode = productNodes.LastOrDefault();
                if (productNode == null)
                {
                    continue;
                }

                var productContentLink = _referenceConverter.GetContentLink(amazonProduct.Details.Asin, CatalogContentType.CatalogEntry);
                if (!ContentReference.IsNullOrEmpty(productContentLink))
                {
                    continue;
                }

                var product = GetProduct(productNode.ContentLink, amazonProduct);
                if (amazonProduct.Details.Images != null && amazonProduct.Details.Images.Any())
                {
                    product.CommerceMediaCollection = GetImages(amazonProduct.Details.Images);
                }

                _contentRepository.Save(product, SaveAction.Publish, AccessLevel.NoAccess);
                productCount++;
                var variants = new List<GenericVariant>();
                foreach (var amazonVariation in amazonProduct.Variations.Variations)
                {
                    var variant = GetVariant(productNode.ContentLink, amazonVariation);
                    if (amazonVariation.Images != null && amazonVariation.Images.Any())
                    {
                        variant.CommerceMediaCollection = GetImages(amazonVariation.Images);
                    }
                    _contentRepository.Save(variant, SaveAction.Publish, AccessLevel.NoAccess);

                    variants.Add(variant);
                    productVariants.Add(new ProductVariation
                    {
                        Parent = product.ContentLink,
                        Child = variant.ContentLink,
                        GroupName = "default",
                        Quantity = 1,
                        SortOrder = 0
                    });

                    productCount++;
                    var inventoryRecords = warehouses.Select(warehouse => new InventoryRecord
                    {
                        AdditionalQuantity = 0,
                        BackorderAvailableQuantity = 0,
                        BackorderAvailableUtc = DateTime.UtcNow,
                        BackorderRequestedQuantity = 0,
                        CatalogEntryCode = variant.Code,
                        PreorderAvailableQuantity = 0,
                        IsTracked = true,
                        PreorderAvailableUtc = DateTime.UtcNow,
                        PreorderRequestedQuantity = 0,
                        PurchaseAvailableQuantity = _random.Next(1000),
                        PurchaseAvailableUtc = DateTime.UtcNow,
                        PurchaseRequestedQuantity = 0,
                        ReorderMinQuantity = 15,
                        WarehouseCode = warehouse.Code
                    }).ToList();

                    _inventoryService.Insert(inventoryRecords);

                    var price = Convert.ToDecimal(_random.Next(10, 200));
                    if (!string.IsNullOrEmpty(amazonVariation.ListPrice?.FormattedPrice))
                    {
                        decimal.TryParse(amazonVariation.ListPrice.FormattedPrice,
                            NumberStyles.Currency,
                            CultureInfo.GetCultureInfo("en-US"),
                            out price);
                    }

                    if (price == 0)
                    {
                        price = Convert.ToDecimal(_random.Next(10, 200));
                    }
                    var prices = new List<PriceDetailValue>
                    {
                        new PriceDetailValue
                        {
                            CatalogKey = new CatalogKey(variant.Code),
                            CustomerPricing = CustomerPricing.AllCustomers,
                            MarketId = new MarketId("US"),
                            MinQuantity = 0,
                            UnitPrice = new Money(price, Currency.USD),
                            ValidFrom = DateTime.UtcNow
                        }
                    };

                    _priceDetailService.Save(prices);


                }
                _relationRepository.UpdateRelations(productVariants);

                var updateProduct = false;
                if (product.Description == null || product.Description.IsEmpty)
                {
                    var description = variants.FirstOrDefault(x => x.Description != null && !x.Description.IsEmpty);
                    if (description != null)
                    {
                        product.Description = description.Description;
                        updateProduct = true;
                    }
                }

                if (product.CommerceMediaCollection == null || !product.CommerceMediaCollection.Any())
                {
                    var media = variants.FirstOrDefault(
                        x => x.CommerceMediaCollection != null && x.CommerceMediaCollection.Any());
                    if (media != null)
                    {
                        product.CommerceMediaCollection = new ItemCollection<CommerceMedia>(new[] { media.CommerceMediaCollection.First().Clone() as CommerceMedia });
                        updateProduct = true;
                    }
                }

                if (updateProduct)
                {
                    _contentRepository.Save(product, SaveAction.Publish, AccessLevel.NoAccess);
                }
            }
            return productCount;
        }

        private GenericProduct GetProduct(ContentReference parentLink, AmazonProduct amazonProduct)
        {
            var product = _contentRepository.GetDefault<GenericProduct>(parentLink);
            product.Name = amazonProduct.Details.Title.Length > 100 ? amazonProduct.Details.Title.Substring(0, 100) : amazonProduct.Details.Title;
            product.DisplayName = amazonProduct.Details.Title;

            if (amazonProduct.Details.Actor != null)
            {
                product.Actor = new ItemCollection<string>(amazonProduct.Details.Actor);
            }

            if (amazonProduct.Details.Artist != null)
            {
                product.Artist = new ItemCollection<string>(amazonProduct.Details.Artist);
            }

            product.Asin = product.Code = amazonProduct.Details.Asin;

            if (amazonProduct.Details.AudioFormat != null)
            {
                product.AudioFormat = new ItemCollection<string>(amazonProduct.Details.AudioFormat);
            }

            if (amazonProduct.Details.Author != null)
            {
                product.Author = new ItemCollection<string>(amazonProduct.Details.Author);
            }

            product.Brand = amazonProduct.Details.Brand;
            product.CeroAgeRating = amazonProduct.Details.CeroAgeRating;
            product.Department = amazonProduct.Details.Department;

            if (amazonProduct.Details.Feature != null)
            {
                var html = amazonProduct.Details.Feature.Aggregate("<ul>", (current, feature) => current + $"<li>{feature}</li>");
                html += "</ul>";
                product.Description = new XhtmlString(html);
            }

            if (amazonProduct.Details.Director != null)
            {
                product.Director = new ItemCollection<string>(amazonProduct.Details.Director);
            }

            product.EpisodeSequence = amazonProduct.Details.EpisodeSequence;
            product.EsrbAgeRating = amazonProduct.Details.EsrbAgeRating;

            if (amazonProduct.Details.Format != null)
            {
                product.Format = new ItemCollection<string>(amazonProduct.Details.Format);
            }

            product.Genre = amazonProduct.Details.Genre;
            product.HazardousMaterialType = amazonProduct.Details.HazardousMaterialType;
            product.IsAdultProduct = amazonProduct.Details.IsAdultProduct;
            product.IsAutographed = amazonProduct.Details.IsAutographed;
            product.IsMemorabilia = amazonProduct.Details.IsMemorabilia;
            product.Label = amazonProduct.Details.Label;
            product.LegalDisclaimer = amazonProduct.Details.LegalDisclaimer;
            product.Manufacturer = amazonProduct.Details.Manufacturer;
            product.ManufacturerPartsWarrantyDescription = amazonProduct.Details.ManufacturerPartsWarrantyDescription;
            product.Model = amazonProduct.Details.Model;
            product.ModelYear = amazonProduct.Details.ModelYear;

            if (amazonProduct.Details.PictureFormat != null)
            {
                product.PictureFormat = new ItemCollection<string>(amazonProduct.Details.PictureFormat);
            }

            if (amazonProduct.Details.Platform != null)
            {
                product.Platform = new ItemCollection<string>(amazonProduct.Details.Platform);
            }

            product.ProductGroup = amazonProduct.Details.ProductGroup;
            product.ProductTypeName = amazonProduct.Details.ProductTypeName;
            product.ProductTypeSubcategory = amazonProduct.Details.ProductTypeSubcategory;
            product.Publisher = amazonProduct.Details.Publisher;
            product.ReleaseDate = amazonProduct.Details.ReleaseDate;

            if (amazonProduct.Details.RunningTime != null)
            {
                product.RunningTime = amazonProduct.Details.RunningTime.ToString();
            }

            product.Studio = amazonProduct.Details.Studio;
            product.Warranty = amazonProduct.Details.Warranty;
            return product;
        }

        private GenericVariant GetVariant(ContentReference parentLink, AmazonItemDetails amazonVariant)
        {
            var variant = _contentRepository.GetDefault<GenericVariant>(parentLink);
            variant.Name = amazonVariant.Title.Length > 100 ? amazonVariant.Title.Substring(0, 100) : amazonVariant.Title;
            variant.DisplayName = amazonVariant.Title;
            variant.Code = amazonVariant.Asin;
            variant.AspectRatio = amazonVariant.AspectRatio;
            variant.AudienceRating = amazonVariant.AudienceRating;
            variant.Binding = amazonVariant.Binding;
            variant.Size = amazonVariant.ClothingSize ?? amazonVariant.Size;
            variant.Color = amazonVariant.Color;

            if (amazonVariant.Feature != null)
            {
                var html = amazonVariant.Feature.Aggregate("<ul>", (current, feature) => current + $"<li>{feature}</li>");
                html += "</ul>";
                variant.Description = new XhtmlString(html);
            }

            variant.Ean = amazonVariant.Ean;
            variant.Edition = amazonVariant.Edition;
            variant.HardwarePlatform = amazonVariant.HardwarePlatform;
            variant.IsEligibleForTradeIn = amazonVariant.IsEligibleForTradeIn;
            variant.Isbn = amazonVariant.Isbn;
            variant.IssuesPerYear = amazonVariant.IssuesPerYear;
            variant.ItemPartNumber = amazonVariant.ItemPartNumber;
            variant.MagazineType = amazonVariant.MagazineType;
            variant.MediaType = amazonVariant.MediaType;
            variant.Mpn = amazonVariant.Mpn;
            variant.NumberOfDiscs = amazonVariant.NumberOfDiscs;
            variant.NumberOfIssues = amazonVariant.NumberOfIssues;
            variant.NumberOfItems = amazonVariant.NumberOfItems;
            variant.NumberOfTracks = amazonVariant.NumberOfTracks;
            variant.NumberOfPages = amazonVariant.NumberOfPages;
            variant.OperatingSystem = amazonVariant.OperatingSystem;
            variant.PackageQuantity = amazonVariant.PackageQuantity;
            variant.PartNumber = amazonVariant.PartNumber;
            variant.PublicationDate = amazonVariant.PublicationDate;
            variant.RegionCode = amazonVariant.RegionCode;
            variant.Sku = amazonVariant.Sku;

            if (amazonVariant.SubscriptionLength != null)
            {
                variant.SubscriptionLength = amazonVariant.SubscriptionLength.ToString();
            }
            variant.TrackSequence = amazonVariant.TrackSequence;
            variant.Upc = amazonVariant.Upc;
            variant.MinQuantity = 0;
            variant.MaxQuantity = 100;
            variant.TrackInventory = true;

            if (amazonVariant.ItemDimensions == null)
            {
                return variant;
            }

            if (amazonVariant.ItemDimensions.Height != null)
            {
                variant.ShippingDimensions.Height = Convert.ToDouble(amazonVariant.ItemDimensions.Height.Value);
            }

            if (amazonVariant.ItemDimensions.Length != null)
            {
                variant.ShippingDimensions.Length = Convert.ToDouble(amazonVariant.ItemDimensions.Length.Value);
            }

            if (amazonVariant.ItemDimensions.Weight != null)
            {
                variant.Weight = Convert.ToDouble(amazonVariant.ItemDimensions.Weight.Value);
            }

            if (amazonVariant.ItemDimensions.Width != null)
            {
                variant.ShippingDimensions.Width = Convert.ToDouble(amazonVariant.ItemDimensions.Width.Value);
            }
            return variant;
        }

        private ItemCollection<CommerceMedia> GetImages(List<AmazonImage> images)
        {
            var media = new List<CommerceMedia>();
            var primary = images.FirstOrDefault(
                x => x.GroupName.Equals("Primary", StringComparison.InvariantCultureIgnoreCase));
            if (primary != null)
            {
                var imageMedia = _contentRepository.GetDefault<ImageMediaData>(_contentImageFolder);
                imageMedia.Name = AddBlobByExternalUri(imageMedia, primary.Url);
                if (!string.IsNullOrEmpty(imageMedia.Name))
                {
                    _contentRepository.Save(imageMedia, SaveAction.Publish, AccessLevel.NoAccess);
                    media.Add(new CommerceMedia(imageMedia.ContentLink, "episerver.core.icontentimage", "default", 0));
                }

            }

            var index = 0;
            foreach (var additionalImage in images.Where(x => !x.GroupName.Equals("Primary", StringComparison.InvariantCultureIgnoreCase)))
            {
                index = index++;
                var imageMedia = _contentRepository.GetDefault<ImageMediaData>(_contentImageFolder);
                imageMedia.Name = AddBlobByExternalUri(imageMedia, additionalImage.Url);
                if (string.IsNullOrEmpty(imageMedia.Name))
                {
                    continue;
                }
                _contentRepository.Save(imageMedia, SaveAction.Publish, AccessLevel.NoAccess);
                media.Add(new CommerceMedia(imageMedia.ContentLink, "episerver.core.icontentimage", "default", index));
            }
            return new ItemCollection<CommerceMedia>(media);
        }

        private string AddBlobByExternalUri(MediaData media, string url)
        {
            Stream stream = null;
            try
            {
                var uri = new Uri(url);
                var name = Path.GetFileName(uri.AbsoluteUri);
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                var fileExtension = Path.GetExtension(uri.AbsoluteUri);
                if (string.IsNullOrEmpty(fileExtension))
                {
                    fileExtension = ".blob";
                }
                if (uri.IsUnc)
                {
                    stream = new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    stream = request.GetResponse().GetResponseStream();
                }
                var blob = _blobFactory.CreateBlob(media.BinaryDataContainer, fileExtension);
                using (var content = stream)
                {
                    blob.Write(content);
                }

                media.BinaryData = blob;
                return name;
            }
            catch (Exception)
            {
                media.BinaryData = null;
                stream?.Dispose();
                return null;
            }
        }

        private ContentReference GetOrCreateContentFolder(string contentFolderUri)
        {
            var contentReference = ContentReference.EmptyReference;
            if (string.IsNullOrEmpty(contentFolderUri))
            {
                return contentReference;
            }
            var segments = contentFolderUri.Split('/').ToList();
            var segmentsReferences = new List<ContentReference>();
            var parentReference = _mediaRootFolder;
            for (var i = 0; i < segments.Count; i++)
            {
                if (i > 0)
                {
                    parentReference = segmentsReferences[i - 1];
                }

                var currentSegment = segments[i];
                ContentReference currentReference;
                var folder = _contentRepository.GetChildren<ContentFolder>(parentReference).FirstOrDefault(x => x.Name == currentSegment);
                if (folder == null)
                {
                    var contentResourceFolder = _contentRepository.GetDefault<ContentFolder>(parentReference);
                    contentResourceFolder.Name = currentSegment;
                    currentReference = _contentRepository.Save(contentResourceFolder, SaveAction.Publish, AccessLevel.NoAccess);
                }
                else
                {
                    currentReference = folder.ContentLink;
                }
                segmentsReferences.Add(currentReference);
            }
            if (segmentsReferences.Count > 0)
            {
                contentReference = segmentsReferences.Last();
            }
            return contentReference;
        }

    }
}
