﻿using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Find;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Pages
{
    [ContentType(
        DisplayName = "Destination",
        GUID = "ac26ee4b-104f-4719-8aab-ad6d3fcb0d75",
        GroupName = "Destinations",
        Description = "Used to display the details of a destination")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-27.png")]
    public class DestinationPage : SitePageData
    {
        [Display(
            Name = "Image",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        [Required]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Image { get; set; }

        [Display(Name = "Tags", Description = "",
            GroupName = SystemTabNames.Content)]
        [AllowedTypes(typeof(TagPage))]
        public virtual ContentArea Tags { get; set; }

        [Display(
            Name = "Continent",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 130)]
        [BackingType(typeof(PropertyString))]
        [Required]
        public virtual string Continent { get; set; }

        [Display(
            Name = "Country",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 140)]
        [BackingType(typeof(PropertyString))]
        [Required]
        public virtual string Country { get; set; }

        [Display(
            Name = "Airport initials",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 150)]
        [BackingType(typeof(PropertyString))]
        public virtual string AirportInitials { get; set; }

        [Display(
            Name = "Yearly passengers",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 160)]
        public virtual int YearlyPassengers { get; set; }

        [Display(
            Name = "Latitude",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 170)]
        [Required]
        public virtual double Latitude { get; set; }

        [Display(
            Name = "Longitude",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 180)]
        [Required]
        public virtual double Longitude { get; set; }

        [Display(
            Name = "Average temperature",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 190)]
        public virtual double? AvgTemp { get; set; }

        [Display(
            Name = "Main Area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 200)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(
            Name = "Sidebar Area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 200)]
        public virtual ContentArea SidebarContentArea { get; set; }

        [Display(Name = "Promoted Destination", 
            Description = "Check this, in order to boost this destination and promote it in suggestions",
            GroupName = SystemTabNames.Content,
            Order = 215)]
        public virtual bool Promoted { get; set; }

        [Display(
            Name = "Is new?",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 210)]
        public virtual bool New { get; set; }

        [Display(
            Name = "Intro text",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        [StringLength(5000)]
        [UIHint(UIHint.Textarea)]
        public virtual string MainIntro { get; set; }

        [Ignore]
        public double AvgTempDbl
        {
            get
            {
                return AvgTemp ?? double.NaN;
            }
        }

        [Ignore]
        public string SearchHitTypeName
        {
            get { return "Destination"; }
        }

        [Ignore]
        public GeoLocation Coordinates
        {
            get
            {
                return new GeoLocation(Latitude, Longitude);
            }
        }

        public List<string> TagString()
        {
            var repo = ServiceLocation.ServiceLocator.Current.GetInstance<IContentRepository>();
            return Tags?.FilteredItems.Select(cai => repo.Get<TagPage>(cai.ContentLink).Name).ToList();
        }
    }
}