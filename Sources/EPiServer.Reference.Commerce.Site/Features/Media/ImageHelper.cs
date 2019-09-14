using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;
using EPiServer.ServiceLocation;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Media
{
    [ServiceConfiguration]
    public class ImageHelper
    {
        private readonly IContentRepository _contentRepository;
        private readonly ILogger _log = LogManager.GetLogger();
        private readonly VisionServiceClient _visionServiceClient;

        public ImageHelper(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
            _visionServiceClient = new VisionServiceClient(ConfigurationManager.AppSettings["Vision:Key"], ConfigurationManager.AppSettings["Vision:ApiRoot"]);
        }

        public async Task TagImagesAsync(ImageMediaData imageFile)
        {
            var analysisResult = await UploadAndAnalyzeImageAsync(imageFile);
            ProcessAnalysisResult(analysisResult, imageFile);
        }

        private void ProcessAnalysisResult(AnalysisResult result, ImageMediaData imageFile)
        {
            if (result == null || imageFile == null)
            {
                return;
            }

            if (result.ImageType != null)
            {
                string clipArtType;
                switch (result.ImageType.ClipArtType)
                {
                    case 0:
                        clipArtType = "Non-clipart";
                        break;
                    case 1:
                        clipArtType = "Ambiguous";
                        break;
                    case 2:
                        clipArtType = "Normal clipart";
                        break;
                    case 3:
                        clipArtType = "Good clipart";
                        break;
                    default:
                        clipArtType = "Unknown";
                        break;
                }

                imageFile.ClipArtType = clipArtType;

                string lineDrawingType;
                switch (result.ImageType.LineDrawingType)
                {
                    case 0:
                        lineDrawingType = "Non-lineDrawing";
                        break;
                    case 1:
                        lineDrawingType = "LineDrawing";
                        break;
                    default:
                        lineDrawingType = "Unknown";
                        break;
                }

                imageFile.LineDrawingType = lineDrawingType;
            }

            if (result.Adult != null)
            {
                imageFile.IsAdultContent = result.Adult.IsAdultContent;
                imageFile.IsRacyContent = result.Adult.IsRacyContent;
            }

            if (result.Categories != null && result.Categories.Length > 0)
            {
                imageFile.ImageCategories = result.Categories.Select(c => c.Name).ToArray();
            }

            if (result.Color != null)
            {
                imageFile.AccentColor = result.Color.AccentColor;
                imageFile.DominantColorBackground = result.Color.DominantColorBackground;
                imageFile.DominantColorForeground = result.Color.DominantColorForeground;
                imageFile.IsBwImg = result.Color.IsBWImg;

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    imageFile.DominantColors = result.Color.DominantColors;
                }
            }

            if ((imageFile.Tags == null || imageFile.Tags.Count == 0) && result.Tags != null)
            {
                imageFile.Tags = result.Tags.Where(t => t.Confidence > 0.5).Select(t => t.Name).ToArray();
            }

            if (result.Description != null)
            {
                imageFile.Caption = result.Description.Captions.OrderByDescending(c => c.Confidence).FirstOrDefault()?.Text;

                if (imageFile.Tags == null || imageFile.Tags.Count == 0)
                {
                    imageFile.Tags = result.Description.Tags;
                }
            }
        }

        private Task<AnalysisResult> UploadAndAnalyzeImageAsync(ImageData imageData)
        {
            _log.Information("VisionServiceClient is created");
            _log.Information("Calling VisionServiceClient.AnalyzeImageAsync()...");
            VisualFeature[] visualFeatures =
            {
                VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color,
                VisualFeature.Description, VisualFeature.ImageType,
                VisualFeature.Tags
            };

            return _visionServiceClient.AnalyzeImageAsync(imageData.BinaryData.OpenRead(), visualFeatures);

        }
    }
}