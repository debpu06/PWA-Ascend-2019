using EPiServer.Reference.Commerce.Site.Features.Loyalty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Loyalty
{
    public interface ILoyaltyService
    {
        void AddNumberOfOrders();
        void AddNumberOfReviews();
    }
}
