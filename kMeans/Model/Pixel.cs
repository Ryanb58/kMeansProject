using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace kMeans.Model
{
    public class Pixel
    {
        public int posX { get; set; }
        public int posY { get; set; }
        public int cluster { get; set; }
        public Color rgb { get; set; }
    }
}
