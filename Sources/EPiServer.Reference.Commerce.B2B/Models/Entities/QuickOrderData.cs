using FileHelpers;

namespace EPiServer.Reference.Commerce.B2B.Models.Entities
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines]
    public class QuickOrderData
    {
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Sku;
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public int Quantity;
    }
}
