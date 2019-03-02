using Accord.Imaging.Filters;
using Accord.Imaging.Textures;
using Accord.Vision.Detection;
using Sitecore.Configuration;
using Sitecore.SecurityModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    public enum EnumImageFilter { Median, GrayScale, Invert, TexturedHue, TextureSepia, Rotate, Resize };
    public class ImagePreprocessing
    {
        public static Bitmap ApplyFilter(ImageFilterParameters imageFilterParameters)
        {
            dynamic filterProcessor = null;
            switch (imageFilterParameters.filter)
            {
                case EnumImageFilter.GrayScale:
                    filterProcessor = new Grayscale(0.2125, 0.7154, 0.0721);
                    break;
                case EnumImageFilter.Invert:
                    filterProcessor = new Invert();
                    break;
                case EnumImageFilter.Median:
                    filterProcessor = new Median();
                    break;
                case EnumImageFilter.Rotate:
                    var finalAngle = imageFilterParameters.Angle.HasValue ? imageFilterParameters.Angle.Value : 0;
                    filterProcessor = new RotateBicubic(finalAngle);
                    break;
                case EnumImageFilter.TexturedHue:
                    filterProcessor = new TexturedFilter(new CloudsTexture(), new HueModifier(50));
                    break;
                case EnumImageFilter.Resize:
                    var finalWidth = imageFilterParameters.Width.HasValue ? imageFilterParameters.Width.Value : imageFilterParameters.Image.Width;
                    var finalHeight = imageFilterParameters.Heigh.HasValue ? imageFilterParameters.Heigh.Value : imageFilterParameters.Image.Height;
                    filterProcessor = new ResizeBicubic(finalWidth, finalHeight);
                    break;
                default:
                    filterProcessor = new TexturedFilter(new CloudsTexture(), new GrayscaleBT709(), new Sepia());
                    break;
            }
            var response = filterProcessor.Apply(imageFilterParameters.Image);
            return response;
        }

        public static FaceDetectionResult FaceDetection(Bitmap image)
        {
            var result = new FaceDetectionResult();
            
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();

            var detector = new HaarObjectDetector(cascade, minSize: 100,
                searchMode: ObjectDetectorSearchMode.NoOverlap);

            
            Bitmap bmp = Accord.Imaging.Image.Clone(image);

            
            Rectangle[] faces = detector.ProcessFrame(bmp);

            var objectMarker = new RectanglesMarker(Color.Red);
            objectMarker.Rectangles = faces;



            var resultImage = objectMarker.Apply(image); // overwrite the frame

            result.ImageResult = resultImage;
            result.TotalFacesDetected = (faces == null) ? 0 : faces.Length;
            result.Rectangles = faces;
            return result;
        }

        public static Bitmap AddTextToImage(Bitmap bmp, Rectangle rectangle, string text)
        {


            RectangleF rectf = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(text, new Font("Tahoma", 14), Brushes.Black, rectf);

            g.Flush();

            return bmp;
        }

        public static void SaveImage(Bitmap image, string fileName, string alternateText)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            
            var options = new Sitecore.Resources.Media.MediaCreatorOptions
            {
                FileBased = false,
                OverwriteExisting = true,
                Versioned = true,
                IncludeExtensionInItemName = true,
                Destination = fileName,
                Database = Factory.GetDatabase("master"),
                AlternateText = alternateText
            };


            using (new SecurityDisabler())
            {
                var creator = new Sitecore.Resources.Media.MediaCreator();
                creator.CreateFromStream(memoryStream, fileName, options);
            }
        }
    }
}