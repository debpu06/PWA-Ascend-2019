using System.Collections.Generic;
using EPiServer.Reference.Commerce.B2B.Models.Search;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Services
{
    public interface IEPiFindSearchService
    {
        IEnumerable<UserSearchResultModel> SearchUsers(string query, int page = 1, int pageSize = 50);
        IEnumerable<SkuSearchResultModel> SearchSkus(string query);
    }
}