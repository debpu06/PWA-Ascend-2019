using System.Linq;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    public class ContentImageAttribute : ImageUrlAttribute
    {
        public ContentImageAttribute() : base("~/Content/ContentIcons/default.png") { }

        public ContentImageAttribute(string path) : base((path.Contains('/')) ? path : "~/Content/ContentIcons/" + path) { }
    }
}