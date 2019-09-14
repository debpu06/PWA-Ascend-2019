using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.SalesRep
{
    public class SalesRepViewModel : ContentViewModel<SalesRepPage>
    {
        public SalesRepViewModel()
        {
            
        }

        public SalesRepViewModel(SalesRepPage page) : base(page)
        {
            
        }

        public List<OrganizationModel> Organizations { get; set; }
    }
}