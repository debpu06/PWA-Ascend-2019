using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Data.Entity;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true)]
    public class NotificationBlockController : BlockController<NotificationBlock>
    {
        private ContentRepository _contentRepository;
        public NotificationBlockController(ContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public override ActionResult Index(NotificationBlock currentBlock)
        {
            var viewModel = new NotificationBlockViewModel(currentBlock);
            return PartialView("~/Views/Blocks/NotificationBlock.cshtml", viewModel);
        }

        public bool Acknowledged(Guid contentGuid)
        {
            var content = _contentRepository.Get<IContent>(contentGuid);
            if (!(((IReadOnly)content)?.CreateWritableClone() is NotificationBlock clone))
            {
                return false;
            }
            var customer = CustomerContext.Current.CurrentContact;
            if (clone.Acknowledgements == null)
            {
                if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
                {
                    clone.Acknowledgements = new List<UserAcknowledgement>
                    {
                        new UserAcknowledgement
                        {
                            Email = customer.Email,
                            Acknowledged = true
                        }
                    };
                }
            }
            else
            {
                if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
                {
                    var match = clone.Acknowledgements.FirstOrDefault(x => x.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        match.Acknowledged = true;
                    }
                    else
                    {
                        clone.Acknowledgements.Add(new UserAcknowledgement
                        {
                            Email = customer.Email,
                            Acknowledged = true
                        });
                    }
                }
            }
            clone.Property["Acknowledgements"].IsModified = true;
            _contentRepository.Save(clone as IContent, DataAccess.SaveAction.Publish, AccessLevel.NoAccess);
            return true;
        }
    }
}
