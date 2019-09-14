using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class NotificationBlockViewModel : IBlockViewModel<NotificationBlock>
    {
        public NotificationBlockViewModel(NotificationBlock block)
        {
            CurrentBlock = block;
        }
        public NotificationBlock CurrentBlock { get; }
    }
}