using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Framework.Serialization.Internal;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure
{
    [PropertyDefinitionTypePlugIn]
    public class SwitchableUserListProperty : PropertyList<SwitchableUser>
    {
        private readonly IObjectSerializer _objectSerializer;
        private Injected<ObjectSerializerFactory> _objectSerializerFactory;

        public SwitchableUserListProperty()
        {
            this._objectSerializer = this._objectSerializerFactory.Service.GetSerializer("application/json");
        }
    }
}