using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.Social.Adapters.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Handlers;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Comments;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Common;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Groups;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Moderation;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Ratings;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.ServiceLocation;
using EPiServer.Social.ActivityStreams.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StructureMap;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    /// <summary>
    /// The SocialInitialization class initializes the IOC container mapping social component
    /// interfaces to their implementations.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(SiteInitialization))]
    public class SocialInitialization : IConfigurableModule
    {
        /// <summary>
        /// Configure the IoC container before initialization.
        /// </summary>
        /// <param name="context">The context on which the container can be accessed.</param>
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.StructureMap().Configure(ConfigureContainer);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="context">The instance context.</param>
        public void Initialize(InitializationEngine context)
        {
            IActivityService activityService = context.Locate.Advanced.GetInstance<IActivityService>();
            activityService.Watch(new ContentRatedActivityHandler());
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        /// <param name="context">The instance context.</param>
        public void Uninitialize(InitializationEngine context)
        {
        }

        /// <summary>
        /// Configure the IOC container.
        /// </summary>
        /// <param name="configuration">The IOC container configuration.</param>
        private static void ConfigureContainer(ConfigurationExpression configuration)
        {
            configuration.For<IUserRepository>().Use(() => CreateUserRepository());
            configuration.For<IPageRepository>().Use<PageRepository>();
            configuration.For<IPageCommentRepository>().Use<PageCommentRepository>();
            configuration.For<IPageRatingRepository>().Use<PageRatingRepository>();
            configuration.For<IPageSubscriptionRepository>().Use<PageSubscriptionRepository>();
            configuration.For<ICommunityActivityAdapter>().Use<CommunityActivityAdapter>();
            configuration.For<ICommunityFeedRepository>().Use<CommunityFeedRepository>();
            configuration.For<ICommunityActivityRepository>().Use<CommunityActivityRepository>();
            configuration.For<ICommunityRepository>().Use<CommunityRepository>();
            configuration.For<ICommunityMemberRepository>().Use<CommunityMemberRepository>();
            configuration.For<ICommunityMembershipModerationRepository>().Use<CommunityMembershipModerationRepository>();
            configuration.For<IReviewActivityService>().Use<ReviewActivityService>();
        }

        /// <summary>
        /// Create a UserRepository.
        /// </summary>
        /// <returns>The created UserRepository instance.</returns>
        private static IUserRepository CreateUserRepository()
        {
            return new UserRepository(new UserManager<IdentityUser>(
                    new UserStore<IdentityUser>(new ApplicationDbContext<IdentityUser>("EcfSqlConnection")))
            );
        }
    }
}