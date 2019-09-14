using EPiServer.Shell.ObjectEditing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Helper
{
    public class FontAwesomeSelectionFactory : ISelectionFactory
    {
        private static IEnumerable<ISelectItem> allFonts;

        /// <summary>
        /// Adapts all font information for dropdownlist to display.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {            
            var fontNames = GetFontAwesomeNames();

            if (allFonts == null)
            {
                allFonts = fontNames.Select(f => new SelectItem() { Text = f, Value = f });
            }
            return allFonts;
        }

        /// <summary>
        /// Retrieves all font names in css file.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetFontAwesomeNames()
        {
            var fonts = new List<string>();
            var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"Assets\Base\font-awesome\css\font-awesome.css");
            var lines = File.ReadAllLines(path);
            var regex = new Regex("fa-.*:before");

            foreach (string line in lines)
            {
                var match = regex.Match(line.Trim());
                if (match.Success)
                {
                    fonts.Add(match.Value.Replace(":before", ""));
                }
            }

            return fonts.OrderBy(x => x);
        }
    }
}