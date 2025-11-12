using GestPipe.Backend.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class DefaultGesture
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("version_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string VersionId { get; set; }

    [BsonElement("gesture_type_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string GestureTypeId { get; set; }

    [BsonElement("pose_label")]
    public string PoseLabel { get; set; }

    [BsonElement("status")]
    public Dictionary<string, string> Status { get; set; }

    [BsonElement("accuracy")]
    public double Accuracy { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("vector_data")]
    public VectorData VectorData { get; set; }
}
