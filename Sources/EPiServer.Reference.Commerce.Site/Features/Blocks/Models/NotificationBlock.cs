using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.PlugIn;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Notification Block", GUID = "227A3381-D02E-4103-AE04-F1856DDC1309", Description = "Notification Block", GroupName = "Content")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-21.png")]
    public class NotificationBlock : BlockData
    {
        [CultureSpecific]
        [UIHint(UIHint.Textarea)]
        public virtual string Message { get; set; }

        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<UserAcknowledgement>))]
        public virtual IList<UserAcknowledgement> Acknowledgements { get; set; }
    }

    [PropertyDefinitionTypePlugIn]
    public class UserAcknowledgementListProperty : PropertyList<UserAcknowledgement>
    {

    }

    public class UserAcknowledgement
    {
        [EmailAddress]
        public string Email { get; set; }

        public bool Acknowledged { get; set; }
    }

    public class NotificationModel
    {
        public Guid ContentGuid { get; set; }
        public string Message { get; set; }
        public IList<UserAcknowledgement> Acknowledgements { get; set; }
    }
}