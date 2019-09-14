using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Jobs.Models
{
    public class AmazonProduct
    {
        public AmazonItemDetails Details { get; set; }
        public AmazonVariations Variations { get; set; }
        public List<string> NodePath { get; set; }
    }

    public class AmazonItemDetails
    {
        public string Asin { get; set; }
        public List<string> Actor { get; set; }
        public List<string> Artist { get; set; }
        public string AspectRatio { get; set; }
        public string AudienceRating { get; set; }
        public List<string> AudioFormat { get; set; }
        public List<string> Author { get; set; }
        public string Binding { get; set; }
        public string Brand { get; set; }
        public List<string> CatalogNumberList { get; set; }
        public List<string> Category { get; set; }
        public string CeroAgeRating { get; set; }
        public string ClothingSize { get; set; }
        public string Color { get; set; }
        public string Department { get; set; }
        public List<string> Director { get; set; }
        public string Ean { get; set; }
        public List<string> EanList { get; set; }
        public string Edition { get; set; }
        public List<string> Eisbn { get; set; }
        public string EpisodeSequence { get; set; }
        public string EsrbAgeRating { get; set; }
        public List<string> Feature { get; set; }
        public List<string> Format { get; set; }
        public string Genre { get; set; }
        public string HardwarePlatform { get; set; }
        public string HazardousMaterialType { get; set; }
        public bool IsAdultProduct { get; set; }
        public bool IsAutographed { get; set; }
        public string Isbn { get; set; }
        public bool IsEligibleForTradeIn { get; set; }
        public bool IsMemorabilia { get; set; }
        public string IssuesPerYear { get; set; }
        public AmazonItemDimensions ItemDimensions { get; set; }
        public string ItemPartNumber { get; set; }
        public string Label { get; set; }
        public List<AmazonLanguage> Languages { get; set; }
        public string LegalDisclaimer { get; set; }
        public AmazonPrice ListPrice { get; set; }
        public string MagazineType { get; set; }
        public string Manufacturer { get; set; }
        public AmazonDecimalWithUnits ManufacturerMaximumAge { get; set; }
        public AmazonDecimalWithUnits ManufacturerMinimumAge { get; set; }
        public string ManufacturerPartsWarrantyDescription { get; set; }
        public string MediaType { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public string Mpn { get; set; }
        public string NumberOfDiscs { get; set; }
        public string NumberOfIssues { get; set; }
        public string NumberOfItems { get; set; }
        public string NumberOfPages { get; set; }
        public string NumberOfTracks { get; set; }
        public string OperatingSystem { get; set; }
        public AmazonItemDimensions PackageDimensions { get; set; }
        public string PackageQuantity { get; set; }
        public string PartNumber { get; set; }
        public List<string> PictureFormat { get; set; }
        public List<string> Platform { get; set; }
        public string ProductGroup { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductTypeSubcategory { get; set; }
        public string PublicationDate { get; set; }
        public string Publisher { get; set; }
        public string RegionCode { get; set; }
        public string ReleaseDate { get; set; }
        public AmazonDecimalWithUnits RunningTime { get; set; }
        public string SeikodoProductCode { get; set; }
        public string Size { get; set; }
        public string Sku { get; set; }
        public string Studio { get; set; }
        public AmazonIntegerWithUnits SubscriptionLength { get; set; }
        public string Title { get; set; }
        public string TrackSequence { get; set; }
        public AmazonPrice TradeInValue { get; set; }
        public string Upc { get; set; }
        public List<string> UpcList { get; set; }
        public string Warranty { get; set; }
        public AmazonPrice WeeeTaxValue { get; set; }
        public List<AmazonImage> Images { get; set; }
    }

    public class AmazonItemDimensions
    {
        public AmazonDecimalWithUnits Height { get; set; }
        public AmazonDecimalWithUnits Length { get; set; }
        public AmazonDecimalWithUnits Weight { get; set; }
        public AmazonDecimalWithUnits Width { get; set; }
    }

    public class AmazonDecimalWithUnits
    {
        public string Units { get; set; }
        public decimal Value { get; set; }

        public override string ToString()
        {
            return $"{Value} {Units}";
        }
    }

    public class AmazonIntegerWithUnits
    {
        public string Units { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{Value} {Units}";
        }
    }

    public class AmazonPrice
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string FormattedPrice { get; set; }
    }

    public class AmazonLanguage
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string AudioFormat { get; set; }
    }

    public class AmazonVariations
    {
        public int TotalVariations { get; set; }
        public int TotalVariationPages { get; set; }
        public AmazonVariationDimensions VariationDimensions { get; set; }
        public List<AmazonItemDetails> Variations { get; set; }
    }

    public class AmazonVariationDimensions
    {
        public List<string> VariationDimension { get; set; }
    }

    public class AmazonImage
    {
        public string Url { get; set; }
        public AmazonDecimalWithUnits Height { get; set; }
        public AmazonDecimalWithUnits Width { get; set; }
        public string IsVerified { get; set; }
        public string GroupName { get; set; }
        public override string ToString()
        {
            return Url;
        }
    }
}
