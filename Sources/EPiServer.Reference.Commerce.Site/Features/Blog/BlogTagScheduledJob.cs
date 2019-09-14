using EPiServer.Core;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace EPiServer.Reference.Commerce.Site.Features.Blog
{
    [ScheduledPlugIn(DisplayName = "Calculate Blog Tags")]
    public class BlogTagScheduledJob : ScheduledJobBase
    {
        private readonly BlogTagFactory _blogTagFactory;

        public BlogTagScheduledJob(BlogTagFactory blogTagFactory)
        {
            _blogTagFactory = blogTagFactory;
        }

        public override string Execute()
        {
            var tags = _blogTagFactory.CalculateTags(ContentReference.StartPage);
            BlogTagRepository.Instance.SaveTags(tags);
            return "OK";
        }
    }
}
