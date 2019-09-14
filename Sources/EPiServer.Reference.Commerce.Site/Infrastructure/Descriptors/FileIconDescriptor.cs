using System;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    public class FileIconDescriptor<T> : UIDescriptor<T> where T : ContentData
    {
        public FileIconDescriptor()
        {
            Type type = GetType();
            string fileTypeName = type.BaseType.GetGenericArguments()[0].Name;
            IconClass = fileTypeName.Replace("File", "").ToLower() + "Icon";
        }
    }

    [UIDescriptorRegistration]
    public class PdfFileDescriptor : FileIconDescriptor<PDFFile> { }
}
