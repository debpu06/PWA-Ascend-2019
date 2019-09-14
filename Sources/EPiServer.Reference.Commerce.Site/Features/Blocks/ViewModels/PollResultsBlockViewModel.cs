using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class PollResultsBlockViewModel
    {
        public IDictionary<string, int> PollResults { get; set; }
        public string Title { get; set; }

        public PollResultsBlockViewModel()
        {
            PollResults = new Dictionary<string, int>();
        }
    }
}
