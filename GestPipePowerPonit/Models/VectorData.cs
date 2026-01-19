using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class VectorData
    {
        public int[] Fingers { get; set; } // 10 phần tử
        public double MainAxisX { get; set; }
        public double MainAxisY { get; set; }
        public double DeltaX { get; set; }
        public double DeltaY { get; set; }
    }
}
