using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace HackathonWeb.Feature.Media.Pipelines.ML
{
    public class ImageFilterParameters
    {
        public EnumImageFilter filter { get; set; }
        public Bitmap Image { get; set; }
        public int? Angle { get; set; }
        public int? Heigh { get; set; }
        public int? Width { get; set; }
    }
}