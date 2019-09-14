using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Shared
{
    [InitializableModule]
    public class LocalDbInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var dir = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\..\appdata\db\");
            AppDomain.CurrentDomain.SetData("DataDirectory", dir.FullName);
        }

        public void Initialize(InitializationEngine context)
        {
            
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}