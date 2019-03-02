using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    public class FaceDetectionResult
    {
        public Bitmap ImageResult { get; set; }
        public int TotalFacesDetected { get; set; }
        public Rectangle[] Rectangles { get; set; }
    }
}