using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class UserGestureConfig
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GestureTypeId { get; set; }
        public string PoseLabel { get; set; }
        public Dictionary<string, string> Status { get; set; }
        public double Accuracy { get; set; }
        public DateTime UpdateAt { get; set; }
        public VectorData VectorData { get; set; }
    }
}
