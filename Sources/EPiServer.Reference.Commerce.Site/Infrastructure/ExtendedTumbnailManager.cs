using EPiServer.Core.Internal;
using EPiServer.DataAnnotations;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.ImageLibrary;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web;
using EPiServer.Web.Internal;
using System;
using System.Collections.Generic;
using System.IO;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public class ExtendedThumbnailManager : ThumbnailManager
    {
        private readonly IBlobFactory _blobFactory;
        private readonly IMimeTypeResolver _mimeTypeResolver;

        public ExtendedThumbnailManager(IContentRepository contentRepository, IBlobFactory blobFactory, BlobResolver blobResolver,
            IBlobAssigner blobAssigner, ImageServiceOptions imageServiceOptions, IMimeTypeResolver mimeTypeResolver,
            IImageDescriptorPropertyResolver imageDescriptorPropertyResolver)
            : base(contentRepository, blobFactory, blobResolver, blobAssigner, imageServiceOptions, mimeTypeResolver, imageDescriptorPropertyResolver)
        {
            _blobFactory = blobFactory;
            _mimeTypeResolver = mimeTypeResolver;
        }

        public override Blob CreateImageBlob(Blob sourceBlob, string propertyName, ImageDescriptorAttribute descriptorAttribute)
        {
            Validator.ThrowIfNull("sourceBlob", sourceBlob);
            Validator.ThrowIfNullOrEmpty("propertyName", propertyName);
            Validator.ThrowIfNull("descriptorAttribute", descriptorAttribute);

            var uriString = string.Format("{0}{1}_{2}{3}", new object[]
                {
                    Blob.GetContainerIdentifier(sourceBlob.ID).ToString(), 
                    Path.GetFileNameWithoutExtension(sourceBlob.ID.LocalPath), 
                    propertyName, 
                    ".png"
                });
            if (descriptorAttribute is ImageScaleDescriptorAttribute)
                return CreateScaledBlob(new Uri(uriString), sourceBlob, descriptorAttribute as ImageScaleDescriptorAttribute);
            return CreateBlob(new Uri(uriString), sourceBlob, descriptorAttribute.Width, descriptorAttribute.Height);
        }

        private Blob CreateScaledBlob(Uri thumbnailUri, Blob blobSource, ImageScaleDescriptorAttribute qJetImageDescriptorAttribute)
        {
            switch (qJetImageDescriptorAttribute.ScaleMethod)
            {
                case ImageScaleType.Resize:
                    var imgOperation = new ImageOperation(ImageEditorCommand.Resize, qJetImageDescriptorAttribute.Width, qJetImageDescriptorAttribute.Height);
                    return CreateBlob(thumbnailUri, blobSource, new List<ImageOperation> {imgOperation}, _mimeTypeResolver.GetMimeMapping(blobSource.ID.LocalPath));
                case ImageScaleType.ScaleToFit:
                    return CreateBlob(thumbnailUri, blobSource, qJetImageDescriptorAttribute.Width, qJetImageDescriptorAttribute.Height);
                default:
                    var imgOperations = CreateImageOperations(blobSource, qJetImageDescriptorAttribute.Width, qJetImageDescriptorAttribute.Height);
                    
                    return CreateBlob(thumbnailUri, blobSource, imgOperations, _mimeTypeResolver.GetMimeMapping(blobSource.ID.LocalPath));
            }
            
        }

        private IEnumerable<ImageOperation> CreateImageOperations(Blob blobSource, int width, int height)
        {
            var imgOperations = new List<ImageOperation>();
            int orgWidth;
            int orgHeight;
            using (var stream = blobSource.OpenRead())
            {
                var image = System.Drawing.Image.FromStream(stream, false);

                orgWidth = image.Width;
                orgHeight = image.Height;

                image.Dispose();
            }

            var scaleFactor = Math.Max((double)width / orgWidth, (double)height / orgHeight);

            var tempWidth = (int) (orgWidth*scaleFactor);
            var tempHeight = (int) (orgHeight*scaleFactor);
            
            imgOperations.Add(new ImageOperation(ImageEditorCommand.ResizeKeepScale, tempWidth , tempHeight));
            imgOperations.Add(new ImageOperation(ImageEditorCommand.Crop, width, height) { Top = (tempHeight - height)/2, Left = (tempWidth - width)/2});

            return imgOperations;
        }

        /// <summary>
        /// Create a thumbnail image with custom mime type
        /// 
        /// </summary>
        /// <param name="thumbnailUri">The Uri of thumbnail</param><param name="blobSource">The source blob</param><param name="imgOperations">The <see cref="T:EPiServer.ImageLibrary.ImageOperation"/> for generated thumbnail</param><param name="mimeType">A supported mime type, see <seealso cref="T:EPiServer.Web.MimeMapping"/></param>
        /// <returns>
        /// The blob thumbnail
        /// </returns>
        private Blob CreateBlob(Uri thumbnailUri, Blob blobSource, IEnumerable<ImageOperation> imgOperations, string mimeType)
        {
            try
            {
                byte[] buffer;
                using (Stream stream = blobSource.OpenRead())
                {
                    var numArray = new byte[stream.Length];
                    stream.Read(numArray, 0, (int)stream.Length);
                    buffer = ImageService.RenderImage(numArray,
                        imgOperations,
                        mimeType, 1f, 50);
                }
                Blob blob = _blobFactory.GetBlob(thumbnailUri);
                using (Stream stream = blob.OpenWrite())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                return blob;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return blobSource;
            }
            
        }
    }

}