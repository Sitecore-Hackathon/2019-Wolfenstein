using System.Drawing;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    public class FaceDetectionResult
    {
        public Bitmap ImageResult { get; set; }
        public int TotalFacesDetected { get; set; }
        public Rectangle[] Rectangles { get; set; }
    }
}