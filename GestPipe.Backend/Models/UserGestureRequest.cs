using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestPipe.Backend.Models
{
    public class UserGestureRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("user_gesture_config_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserGestureConfigId { get; set; }

        [BsonElement("gesture_type_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GestureTypeId { get; set; }

        [BsonElement("pose_label")]
        public string PoseLabel { get; set; }

        [BsonElement("status")]
        public Dictionary<string, string> Status { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
