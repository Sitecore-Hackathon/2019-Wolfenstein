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
    /// <summary>
    /// This class contains all application filters available
    /// </summary>
    public class ImagePreprocessing
    {
        /// <summary>
        /// Apply a filter over an image
        /// </summary>
        /// <param name="imageFilterParameters">Filter parameters</param>
        /// <returns>Image result of apply filter</returns>
        public static Bitmap ApplyFilter(ImageFilterParameters imageFilterParameters)
        {
            dynamic filterProcessor;
            // choose the filter
            switch (imageFilterParameters.Filter)
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
                    var finalAngle = imageFilterParameters.Angle ?? 0;
                    filterProcessor = new RotateBicubic(finalAngle);
                    break;
                case EnumImageFilter.TexturedHue:
                    filterProcessor = new TexturedFilter(new CloudsTexture(), new HueModifier(50));
                    break;
                case EnumImageFilter.Resize:
                    var finalWidth = imageFilterParameters.Width ?? imageFilterParameters.Image.Width;
                    var finalHeight = imageFilterParameters.Height ?? imageFilterParameters.Image.Height;
                    filterProcessor = new ResizeBicubic(finalWidth, finalHeight);
                    break;
                case EnumImageFilter.FaceDetection:
                    return FaceDetection(imageFilterParameters.Image).ImageResult;
                default:
                    filterProcessor = new TexturedFilter(new CloudsTexture(), new GrayscaleBT709(), new Sepia());
                    break;
            }
            // apply the filter
            var response = filterProcessor.Apply(imageFilterParameters.Image);
            // return new image
            return response;
        }

        /// <summary>
        /// This method helps to face detection
        /// </summary>
        /// <param name="image">Original image</param>
        /// <returns>Face detection result</returns>
        public static FaceDetectionResult FaceDetection(Bitmap image)
        {
            // Creating an face detection instance
            var result = new FaceDetectionResult();
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();
            // set min size in the picture to detect faces
            var detector = new HaarObjectDetector(cascade, minSize: 100, searchMode: ObjectDetectorSearchMode.NoOverlap);
            var bmp = Accord.Imaging.Image.Clone(image);
            // execute detection
            var faces = detector.ProcessFrame(bmp);
            // set rectangle color
            var objectMarker = new RectanglesMarker(Color.Red) { Rectangles = faces };
            // apply filter and get a new image
            var resultImage = objectMarker.Apply(image); // overwrite the frame
            result.ImageResult = resultImage;
            // set result parameters
            result.TotalFacesDetected = faces?.Length ?? 0;
            result.Rectangles = faces;
            return result;
        }

        /// <summary>
        /// Add names to image were is a face detected
        /// </summary>
        /// <param name="bmp">Image with face detection</param>
        /// <param name="rectangle">Rectangle´s information</param>
        /// <param name="text">text to add inside rectangle</param>
        /// <returns></returns>
        public static Bitmap AddTextToImage(Bitmap bmp, Rectangle rectangle, string text)
        {
            // To draw rectangle
            var rectf = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            var g = Graphics.FromImage(bmp);
            // Add text
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(text, new Font("Tahoma", 14), Brushes.Black, rectf);

            g.Flush();
            // return new image
            return bmp;
        }

        /// <summary>
        /// save image inside media gallery
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="fileName">File name</param>
        /// <param name="alternateText">Alternate text image</param>
        public static void SaveImage(Bitmap image, string fileName, string alternateText)
        {
            // Create memory stream object
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            // save image in Media galley
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

        /// <summary>
        /// Convert Bitmap object to byte array
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Byte array</returns>
        public static byte[] ConvertImageToArray(Bitmap image)
        {
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bitmapBytes = memoryStream.ToArray();
            return bitmapBytes;
        }
    }
}