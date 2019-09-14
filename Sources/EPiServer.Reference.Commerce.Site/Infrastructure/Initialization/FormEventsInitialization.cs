﻿using EPiServer.Core;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Events;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FormEventsInitialization : IInitializableModule
    {
        private static readonly ILogger _logger = LogManager.GetLogger();

        public void Initialize(InitializationEngine context)
        {
            var formsEvents = ServiceLocator.Current.GetInstance<FormsEvents>();
            formsEvents.FormsSubmissionFinalized += FormsEvents_FormsSubmissionFinalized;
        }

        private void FormsEvents_FormsSubmissionFinalized(object sender, FormsEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.FormsContent.Name))
            {
                var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
                FormsSubmittedEventArgs submitArgs = e as FormsSubmittedEventArgs;
                string msg = string.Format("Form {0} completed at {1}\tSubmission id: {2}",
                                           e.FormsContent.Name,
                                           DateTime.Now,
                                           submitArgs.SubmissionData.Id);
                _logger.Information(msg);

                var formElements = submitArgs.FormsContent.Property["ElementsArea"].Value as ContentArea;

                foreach (var item in submitArgs.SubmissionData.Data)
                {
                    if (item.Key.StartsWith("SYSTEM"))
                    {
                        _logger.Information(item.Key + ": " + item.Value);
                    }
                    else
                    {
                        int id = Convert.ToInt32(item.Key.Substring(item.Key.LastIndexOf("_") + 1));
                        var elementId = formElements.Items.Where(i => i.ContentLink.ID == id).FirstOrDefault();
                        if (elementId != null)
                        {
                            ElementBlockBase element = loader.Get<ElementBlockBase>(elementId.ContentLink) as ElementBlockBase;
                            string friendlyName = element != null ? element.GetElementInfo().FriendlyName : item.Key;
                            _logger.Information(friendlyName + ": " + item.Value);
                        }
                    }
                }




            }
        }


        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            var formsEvents = ServiceLocator.Current.GetInstance<FormsEvents>();
            formsEvents.FormsSubmissionFinalized -= FormsEvents_FormsSubmissionFinalized; ;

        }
    }
}