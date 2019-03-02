using System.Drawing;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    /// <summary>
    /// Face detection result model
    /// </summary>
    public class FaceDetectionResult
    {
        // Result of the face detection process as bitmap
        public Bitmap ImageResult { get; set; }

        // Number of faces detected
        public int TotalFacesDetected { get; set; }

        // Rectangles to be drawn
        public Rectangle[] Rectangles { get; set; }
    }
}