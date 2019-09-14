using Episerver.Marketing.Connector.Framework;
using Episerver.Marketing.Connector.Framework.Data;
using EPiServer.Campaign.Extensions;
using EPiServer.ConnectForCampaign.Implementation.Services;
using EPiServer.ConnectForCampaign.Services.Implementation;
using EPiServer.Data.Dynamic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignInitialisation
{
    [InitializableModule, ModuleDependency(typeof(MarketingConnectorInitialization))]
    public class InitCampaignFromConfig : IConfigurableModule
    {
        public const string ConfigUsername = "campaign:Username";
        public const string ConfigPassword = "campaign:Password";
        public const string ConfigClientid = "campaign:Clientid";

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<ICampaignSoapService, CampaignSoapService>();
            context.ConfigurationComplete += (o, e) =>
            {
                e.Services.AddTransient<IOptinProcessService, DemoOptinProcessService>();
                e.Services.AddSingleton<IRecipientListService, ExtendedRecipientListService>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
            // Check if the connector has been configured, if not look for settings in config
            var authService = context.Locate.Advanced.GetInstance<IAuthenticationService>();
            var manager = context.Locate.Advanced.GetInstance<IMarketingConnectorManager>();
            var settings = manager.GetConnectorCredentials(ConnectForCampaign.Core.Helpers.Constants.ConnectorId.ToString(),
                ConnectForCampaign.Core.Helpers.Constants.DefaultConnectorInstanceId.ToString());

            if (settings == null || CampaignSettingsFactory.Current.Password == null) 
            {
                // Look for config in the settings
                if (ConfigurationManager.AppSettings[ConfigUsername] != null &&
                    ConfigurationManager.AppSettings[ConfigPassword] != null &&
                    ConfigurationManager.AppSettings[ConfigClientid] != null)
                {
                    var campaignSettings = CampaignSettingsFactory.Current;
                    campaignSettings.UserName = ConfigurationManager.AppSettings[ConfigUsername];
                    campaignSettings.Password = ConfigurationManager.AppSettings[ConfigPassword];
                    campaignSettings.MandatorId = ConfigurationManager.AppSettings[ConfigClientid];
                    campaignSettings.CacheTimeout = 10;

                    if (settings == null)
                    {
                        settings = new ConnectorCredentials()
                        {
                            ConnectorName = ConnectForCampaign.Core.Helpers.Constants.DefaultConnectorName,
                            ConnectorId = ConnectForCampaign.Core.Helpers.Constants.ConnectorId,
                            ConnectorInstanceId = ConnectForCampaign.Core.Helpers.Constants.DefaultConnectorInstanceId,
                            CredentialFields = new Dictionary<string, object>()
                        };
                    }
                    settings.CredentialFields.Add(ConnectForCampaign.Implementation.Helpers.Constants.UsernameFieldKey, campaignSettings.UserName);
                    settings.CredentialFields.Add(ConnectForCampaign.Implementation.Helpers.Constants.PasswordFieldKey, campaignSettings.Password);
                    settings.CredentialFields.Add(ConnectForCampaign.Implementation.Helpers.Constants.MandatorIdFieldKey, campaignSettings.MandatorId);
                    settings.CredentialFields.Add(ConnectForCampaign.Implementation.Helpers.Constants.CacheTimeoutFieldKey, 10);

                    // Test the credentials before saving to database
                    var token = authService.GetToken(
                        settings.CredentialFields[ConnectForCampaign.Implementation.Helpers.Constants.MandatorIdFieldKey] as string,
                        settings.CredentialFields[ConnectForCampaign.Implementation.Helpers.Constants.UsernameFieldKey] as string,
                        settings.CredentialFields[ConnectForCampaign.Implementation.Helpers.Constants.PasswordFieldKey] as string,
                        false);

                    if (string.IsNullOrEmpty(token))
                    {
                        throw new AuthenticationException("Authentication failed");
                    }

                    manager.SaveConnectorCredentials(settings);
                }
            }

            var store = DynamicDataStoreFactory.Instance.GetStore("Marketing_Automation_Settings");
            var globalSettingsList = store.LoadAll<GlobalSettings>().ToList();
            GlobalSettings globalSettings = null;
            if (globalSettingsList.Any() && globalSettingsList.Count > 1)
            {
                globalSettings = globalSettingsList.OrderBy(x => x.LastUpdated).FirstOrDefault();
            }
            else if (globalSettingsList.Any())
            {
                globalSettings = globalSettingsList.FirstOrDefault();
            }
            if (globalSettings == null)
            {
                store.Save(new GlobalSettings
                {
                    EnableFormAutoFill = true,
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}