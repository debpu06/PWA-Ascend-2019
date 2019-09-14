using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Owin;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class NotificationInitialization : IInitializableModule
    {

        public void Initialize(InitializationEngine context)
        {
            //Add initialization logic, this method is called once after CMS has been initialized
            context.Locate.ContentEvents().PublishedContent += contentEvents_PublishedContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
            context.Locate.ContentEvents().PublishedContent -= contentEvents_PublishedContent;
        }

        void contentEvents_PublishedContent(object sender, EPiServer.ContentEventArgs e)
        {
            if (e.Content is BaseStartPage || e.Content is NotificationBlock)
            {
                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                var startPage = contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
                var notifications = startPage.Notifications != null ?
                                    startPage.Notifications.FilteredItems.Select(o => contentLoader.Get<NotificationBlock>(o.ContentLink))
                                    .Select(n => new NotificationModel
                                    {
                                        Acknowledgements = n.Acknowledgements,
                                        Message = n.Message,
                                        ContentGuid = (n as IContent).ContentGuid
                                    }).ToList()
                                    : new List<NotificationModel>();
                var _hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                _hubContext.Clients.All.putNotifications(notifications);
            }
        }
    }
}