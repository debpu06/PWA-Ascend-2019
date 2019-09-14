using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Media;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using System;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ImageTaggingInitializationModule : IInitializableModule
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private static bool _initialized;
        private ImageHelper _imageHelper;

        public void Initialize(InitializationEngine context)
        {
            if (context == null)
            {
                return;
            }

            if (_initialized)
            {
                return;
            }

            _imageHelper = context.Locate.Advanced.GetInstance<ImageHelper>();
            Logger.Information("[ImageTagging] Initializing image tagging functionality.");
            context.Locate.Advanced.GetInstance<IContentEvents>().SavingContent += OnSavingContent;
            _initialized = true;
            Logger.Information("[ImageTagging] Image tagging initialized.");
        }

        public void Uninitialize(InitializationEngine context)
        {
            if (context == null)
            {
                return;
            }

            if (!_initialized)
            {
                return;
            }

            Logger.Information("[ImageTagging] Uninitializing image tagging functionality.");
            context.Locate.Advanced.GetInstance<IContentEvents>().SavingContent -= OnSavingContent;
            Logger.Information("[ImageTagging] Image tagging uninitialized.");
        }

        private void OnSavingContent(object sender, ContentEventArgs contentEventArgs)
        {
            var contentData = contentEventArgs?.Content as ImageMediaData;
            if (contentData == null || ((SaveContentEventArgs)contentEventArgs).Action != DataAccess.SaveAction.Publish)
            {
                return;
            }

            try
            {
                AsyncHelpers.RunSync(() => _imageHelper.TagImagesAsync(contentData));
            }
            catch (Exception e)
            {
               Logger.Error(e.Message, e);
            }
           
        }
    }
}