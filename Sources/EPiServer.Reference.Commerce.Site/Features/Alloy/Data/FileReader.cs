using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Data
{
    public class FileReader
    {
        public static string GetFileSize(MediaData media)
        {
            if (media != null)
            {
                using (var stream = media.BinaryData.OpenRead())
                {
                    return (stream.Length / 1024) + " kB";
                }
            }
            return string.Empty;
        }
    }
}
