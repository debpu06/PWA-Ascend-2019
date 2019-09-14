/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace EPiServer.Reference.Commerce.Site.Features.Channels
{
    /// <summary>
    /// Defines resolution for desktop displays
    /// </summary>
    public class StandardResolution : DisplayResolutionBase
    {
        public StandardResolution() : base("/resolutions/standard", 1366, 768)
        {
        }
    }

    /// <summary>
    /// Defines resolution for a horizontal iPad
    /// </summary>
    public class IpadHorizontalResolution : DisplayResolutionBase
    {
        public IpadHorizontalResolution() : base("/resolutions/ipadhorizontal", 1024, 768)
        {
        }
    }

    /// <summary>
    /// Defines resolution for a vertical iPhone 5s
    /// </summary>
    public class IphoneVerticalResolution : DisplayResolutionBase
    {
        public IphoneVerticalResolution() : base("/resolutions/iphonevertical", 320, 568)
        {
        }
    }

    /// <summary>
    /// Defines resolution for a vertical Android handheld device
    /// </summary>
    public class AndroidVerticalResolution : DisplayResolutionBase
    {
        public AndroidVerticalResolution() : base("/resolutions/androidvertical", 480, 800)
        {
        }
    }
}
