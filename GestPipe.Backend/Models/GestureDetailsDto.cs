using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipe.Backend.Models
{
    public class GestureDetailsDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> Name { get; set; } // Chuẩn đa ngôn ngữ
        public Dictionary<string, string> Type { get; set; }
        public string PoseLabel { get; set; }
        //public string Type { get; set; }
        public double Accuracy { get; set; }
        public Dictionary<string, string> Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Description { get; set; }
        public VectorData VectorData { get; set; }
    }
}
