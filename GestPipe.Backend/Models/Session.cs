using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GestPipe.Backend.Models
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("category_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonElement("topic_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TopicId { get; set; }
        // Sửa lại records thành Dictionary hoặc BsonDocument để lưu gesture count
        [BsonElement("records")]
        public Dictionary<string, int> Records { get; set; }
        // Hoặc:
        // public Dictionary<string, int> Records { get; set; }
        [BsonElement("duration")]
        public double Duration { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}