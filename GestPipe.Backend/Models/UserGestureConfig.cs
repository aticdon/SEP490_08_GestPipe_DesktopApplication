using GestPipe.Backend.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GestPipe.Backend.Models
{
    public class UserGestureConfig
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("gesture_type_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GestureTypeId { get; set; }

        [BsonElement("pose_label")]
        public string PoseLabel { get; set; }

        [BsonElement("status")]
        public Dictionary<string, string> Status { get; set; }

        [BsonElement("accuracy")]
        public double Accuracy { get; set; }

        [BsonElement("update_at")]
        public DateTime UpdateAt { get; set; }

        [BsonElement("vector_data")]
        public VectorData VectorData { get; set; }
    }
}