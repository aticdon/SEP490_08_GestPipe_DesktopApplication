using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestPipe.Backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("status_gesture")]
        public string StatusGesture { get; set; }

        [BsonElement("model")]
        public string Model { get; set; }

        [BsonElement("request_count")]
        public int RequestCount { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("last_login")]
        public DateTime LastLogin { get; set; }

        [BsonElement("current_ver")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CurrentVer { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }
    }
}