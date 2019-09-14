using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class OrderNoteMappings
    {
        public static OrderNote ConvertToOrderNote(
            this Models.OrderNote orderNoteDto, OrderNote orderNote)
        {
            orderNote.CustomerId = orderNoteDto.CustomerId;
            orderNote.Detail = orderNoteDto.Detail;
            orderNote.Title = orderNoteDto.Title;
            orderNote.Type = orderNoteDto.Type;
            orderNote.LineItemId = orderNote.LineItemId;

            return orderNote;
        }

        public static Models.OrderNote ConvertToOrderNote(this OrderNote orderNote)
        {
            return new Models.OrderNote
            {
                CustomerId = orderNote.CustomerId,
                Detail = orderNote.Detail,
                Title = orderNote.Title,
                Type = orderNote.Type,
                LineItemId = orderNote.LineItemId
            };
        }
    }
}