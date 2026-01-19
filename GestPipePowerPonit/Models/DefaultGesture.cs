using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class DefaultGesture
    {
        public string Id { get; set; }

        public string VersionId { get; set; }

        public string GestureTypeId { get; set; }

        public string PoseLabel { get; set; }

        public Dictionary<string, string> Status { get; set; }

        public double Accuracy { get; set; }

        public DateTime CreatedAt { get; set; }
        public VectorData VectorData { get; set; }
    }
}
