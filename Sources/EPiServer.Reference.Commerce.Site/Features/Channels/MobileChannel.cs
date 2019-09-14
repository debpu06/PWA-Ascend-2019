/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web;
using System.Web.WebPages;
using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Channels
{
     //<summary>
     //Defines the 'Mobile' content channel
     //</summary>
    public class MobileChannel : DisplayChannel
    {
        public const string Name = "mobile";

        public override string ChannelName
        {
            get
            {
                return Name;
            }
        }

        public override string ResolutionId
        {
            get
            {
                return typeof(IphoneVerticalResolution).FullName;
            }
        }

        public override bool IsActive(HttpContextBase context)
        {
            return context.GetOverriddenBrowser().IsMobileDevice;
        }
    }
}
