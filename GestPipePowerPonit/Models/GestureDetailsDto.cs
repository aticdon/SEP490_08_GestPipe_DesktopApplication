using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class GestureDetailsDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> Name { get; set; } // multi-language: code
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
